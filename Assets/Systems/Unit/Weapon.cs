using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Cameras;
using TwoBears.Perception;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Perceiver))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CameraShaker))]
    public class Weapon : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 0.5f;
        [SerializeField] private float lifesteal = 0.0f;

        [Header("Layers")]
        [SerializeField] private LayerMask units;
        [SerializeField] private LayerMask weapon;
        [SerializeField] private LayerMask armour;

        //Access
        public int BaseDamage
        {
            get { return damage; }
            set { damage = value; }
        }
        public float DamageMultiplier
        {
            get { return damageMultiplier; }
            set { damageMultiplier = value; }
        }

        public int Damage
        {
            get { return Mathf.CeilToInt(damage * damageMultiplier); }
        }
        public float Lifesteal
        {
            get { return lifesteal; }
            set { lifesteal = value; }
        }
        public float Knockback
        {
            get { return knockback; }
            set { knockback = value; }
        }

        //Buffs
        private float damageMultiplier = 1.0f;

        //Weapon state
        private bool liveEdge;

        //Components
        private Animator anim;
        private Perceiver perceiver;
        private AudioSource audioSource;
        private CameraShaker shaker;
        private TrailRenderer[] trails;

        //Ignore
        private List<Rigidbody2D> ignore;

        //Mono
        private void Awake()
        {
            anim = GetComponent<Animator>();
            perceiver = GetComponent<Perceiver>();
            audioSource = GetComponent<AudioSource>();
            shaker = GetComponent<CameraShaker>();

            ignore = new List<Rigidbody2D>();

            trails = GetComponentsInChildren<TrailRenderer>();
            SetStatus(false);

            liveEdge = false;
        }

        //Physics
        private void OnTriggerStay2D(Collider2D other)
        {
            //Weapon must be active
            if (!liveEdge) return;

            //Ignore
            if (ignore == null || ignore.Contains(other.attachedRigidbody)) return;

            //Play hit
            audioSource.Play();

            //Deactivate on armour hits
            if (armour == (armour | (1 << other.gameObject.layer)))
            {
                //Camera shake
                shaker.Trigger();

                //Add armoured unit to ignore
                ignore.Add(other.attachedRigidbody);
                return;
            }

            //Deactivate & deal damage on unit hits
            if (units == (units | (1 << other.gameObject.layer)))
            {
                //Get unit
                BaseUnit targetUnit = other.GetComponent<BaseUnit>();
                if (targetUnit == null) return;

                //Get perception
                Perceivable target = targetUnit.GetComponent<Perceivable>();
                if (target == null || target.Faction == perceiver.Faction) return;

                //Add hit unit to ignore
                ignore.Add(other.attachedRigidbody);

                //Apply lifesteal
                ApplyLifesteal(targetUnit);

                //Damage unit
                targetUnit.RemoveHealth(Damage);

                //Knock unit back
                Vector3 direction = (other.transform.position - transform.position).normalized;
                Vector2 force = direction * knockback;
                targetUnit.KnockBack(force);

                //Camera shake
                shaker.Trigger();

                //Play hit effect
                targetUnit.TriggerHitParticles(-direction, Damage);
                return;
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            //Grab self
            Collider2D self = collision.collider;
            if ((weapon & (0 << self.gameObject.layer)) != 0) return;

            //Grab other
            Rigidbody2D other = collision.rigidbody;
            if (other == null) return;

            //Ignore
            if (ignore == null || ignore.Contains(collision.rigidbody)) return;

            //Weapon must be active
            if (!liveEdge) return;

            //Play hit
            audioSource.Play();

            //Deactivate on armour hits
            if (armour == (armour | (1 << other.gameObject.layer)))
            {
                //Camera shake
                shaker.Trigger();

                //Add armoured unit to ignore
                ignore.Add(other);

                return;
            }

            //Deactivate & deal damage on unit hits
            if ((units & (1 << other.gameObject.layer)) != 0)
            {
                //Get unit
                BaseUnit targetUnit = other.GetComponent<BaseUnit>();
                if (targetUnit == null) return;

                //Get perception
                Perceivable target = targetUnit.GetComponent<Perceivable>();
                if (target == null || target.Faction == perceiver.Faction) return;

                //Apply lifesteal
                ApplyLifesteal(targetUnit);

                //Damage unit
                targetUnit.RemoveHealth(Damage);

                //Add hit unit to ignore
                ignore.Add(other);

                //Knock unit back
                Vector3 direction = (other.transform.position - transform.position).normalized;
                Vector2 force = direction * knockback;
                targetUnit.KnockBack(force);

                //Camera shake
                shaker.Trigger();

                //Play hit effect
                targetUnit.TriggerHitParticles(-direction, Damage);

                return;
            }
        }

        //Lifesteal
        private void ApplyLifesteal(BaseUnit other)
        {
            //Lifesteal required
            if (lifesteal == 0) return;

            //Grab base unit
            BaseUnit unit = GetComponent<BaseUnit>();

            //Unit must be alive
            if (unit.Health == 0) return;

            //Restore health
            if (other != null && unit != null) unit.RestoreHealth(Mathf.CeilToInt(Mathf.Max(other.Health, Damage) * lifesteal));
        }

        //Status
        private void SetStatus(bool status)
        {
            //Edge 
            liveEdge = status;

            //Setup trails
            if (trails == null || trails.Length == 0) return;
            for (int i = 0; i < trails.Length; i++)
            {
                trails[i].emitting = status;
            }
        }

        //Access
        public virtual void Strike()
        {
            //Play animation
            anim.Play("Attack");

            //Start trail
            SetStatus(true);

            //Initialize ignore
            if (ignore == null) ignore = new List<Rigidbody2D>();
            else ignore.Clear();
        }
        public void StopTrail()
        {
            //Stop trail
            SetStatus(false);
        }
    }
}