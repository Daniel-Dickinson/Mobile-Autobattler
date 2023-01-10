using System.Collections;
using System.Collections.Generic;
using TwoBears.Perception;
using UnityEngine;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Perceiver))]
    [RequireComponent(typeof(AudioSource))]
    public class Weapon : MonoBehaviour
    {
        [Header("Stats")]
        public int damage = 1;
        public float knockback = 0.5f;

        [Header("Layers")]
        [SerializeField] private LayerMask units;
        [SerializeField] private LayerMask weapon;
        [SerializeField] private LayerMask armour;

        [Header("Trail")]
        [SerializeField] private TrailRenderer trail;

        //State
        private bool Attacking
        {
            get
            {
                return anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") || anim.GetNextAnimatorStateInfo(0).IsName("Attack");
            }
        }

        //Components
        private Animator anim;
        private Perceiver perceiver;
        private AudioSource audioSource;

        //Ignore
        private List<Rigidbody2D> ignore;

        //Mono
        private void Awake()
        {
            anim = GetComponent<Animator>();
            perceiver = GetComponent<Perceiver>();
            audioSource = GetComponent<AudioSource>();

            if (trail != null) trail.emitting = false;
        }

        //Physics
        private void OnTriggerStay2D(Collider2D other)
        {
            //Weapon must be active
            if (!Attacking) return;

            //Ignore
            if (ignore.Contains(other.attachedRigidbody)) return;

            //Play hit
            audioSource.Play();

            //Deactivate on armour hits
            if (armour == (armour | (1 << other.gameObject.layer)))
            {
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

                //Damage unit
                targetUnit.RemoveHealth(damage);

                //Knock unit back
                Vector3 direction = (other.transform.position - transform.position).normalized;
                Vector2 force = direction * knockback;

                targetUnit.KnockBack(force);
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
            if (!Attacking) return;

            //Play hit
            audioSource.Play();


            //Deactivate on armour hits
            if (armour == (armour | (1 << other.gameObject.layer)))
            {
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

                //Damage unit
                targetUnit.RemoveHealth(damage);

                //Add hit unit to ignore
                ignore.Add(other);

                //Knock unit back
                Vector3 direction = (other.transform.position - transform.position).normalized;
                Vector2 force = direction * knockback;

                targetUnit.KnockBack(force);
                return;
            }
        }

        //Access
        public virtual void Strike()
        {
            //Play animation
            anim.Play("Attack");

            //Start trail
            if (trail != null) trail.emitting = true;

            //Initialize ignore
            if (ignore == null) ignore = new List<Rigidbody2D>();
            else ignore.Clear();
        }

        //Stop Trail -- Called by attack animation when appropriate
        public void StopTrail()
        {
            //Stop trail
            if (trail != null) trail.emitting = false;
        }
    }
}