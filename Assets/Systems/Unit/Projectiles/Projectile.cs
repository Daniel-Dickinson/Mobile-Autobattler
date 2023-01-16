using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Cameras;
using TwoBears.Perception;
using TwoBears.Pooling;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CameraShaker))]

    public class Projectile : Poolable
    {
        [Header("Stats")]
        [SerializeField] private int damage = 1;

        [Header("Impale")]
        [SerializeField] private float impale = 0.0f;

        [Header("Layers")]
        [SerializeField] private LayerMask units;
        [SerializeField] private LayerMask armour;

        //Access
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        //State
        private Faction faction;
        private List<Rigidbody2D> ignore;
        private BaseUnit launcher;

        //Events
        public HitEvent OnHit;

        //Components
        private Rigidbody2D rb;
        private TrailRenderer trail;
        private AudioSource audioSource;
        private CameraShaker shaker;

        //Mono
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            trail = GetComponent<TrailRenderer>();
            audioSource = GetComponent<AudioSource>();
            shaker = GetComponent<CameraShaker>();

            trail.emitting = false;
        }

        //Poolable
        public override void PoolableInit()
        {
            base.PoolableInit();

            //Clear ignore
            if (ignore != null) ignore.Clear();

            //Clear trail
            if (trail != null) trail.Clear();

            //Enable physics
            rb.simulated = true;
            GetComponent<Collider2D>().enabled = true;
        }

        //Launch
        public void Launch(BaseUnit launcher, Faction faction, Vector3 launchVector)
        {
            //Initialize ignore
            if (ignore == null) ignore = new List<Rigidbody2D>();
            else ignore.Clear();

            //Track faction
            this.faction = faction;
            this.launcher = launcher;

            //Add launcher to ignore list
            ignore.Add(launcher.RigidBody);

            //Add velocity
            rb.velocity = launchVector;

            //Trail now emitting
            if (trail != null) trail.emitting = true;
        }

        //Physics
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Impale or reduce life on hit
            if (impale > 0) Impale(collision.collider, collision.contacts[0].point);
            else lifeRemaining = Mathf.Min(0.1f, lifeRemaining);

            //Ignore
            if (ignore.Contains(collision.rigidbody)) return;

            //Stop trail
            if (trail != null) trail.emitting = false;

            //Play hit
            audioSource.Play();

            //Play on hit
            OnHit?.Invoke(launcher, faction, collision);

            //Ignore the environment
            if (collision.rigidbody == null) return;

            //Deactivate on armour hits
            if (armour == (armour | (1 << collision.rigidbody.gameObject.layer)))
            {
                //Camera shake
                shaker.Trigger();

                //Ignore uint
                ignore.Add(collision.rigidbody);
                return;
            }

            //Deactivate & deal damage on unit hits
            if (units == (units | (1 << collision.rigidbody.gameObject.layer)))
            {
                //Get unit
                BaseUnit otherUnit = collision.rigidbody.GetComponent<BaseUnit>();
                if (otherUnit == null) return;

                //Get perception
                Perceivable perceivable = otherUnit.GetComponent<Perceivable>();
                if (perceivable == null || perceivable.Faction == faction) return;

                //Damage unit
                otherUnit.RemoveHealth(damage);

                //Play hit effect
                otherUnit.TriggerParticles(-transform.forward, damage);

                //Ignore unit
                ignore.Add(collision.rigidbody);

                //Camera shake
                shaker.Trigger();

                return;
            }
        }

        //Impale
        private void Impale(Collider2D col, Vector3 hit)
        {
            //Calculate direction from launcher
            Vector3 direction = (launcher.transform.position - hit).normalized;

            //Disable physics
            rb.simulated = false;
            GetComponent<Collider2D>().enabled = false;

            //Extend life
            lifeRemaining = lifetime * 5.0f;

            //Check if unit
            if (col.attachedRigidbody != null)
            {
                BaseUnit unit = col.attachedRigidbody.GetComponent<BaseUnit>();
                if (unit != null)
                {
                    //Set position centered on unit
                    transform.position = unit.transform.position - (direction * impale);

                    //Set rotation
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);

                    //Parent
                    transform.SetParent(col.transform, true);
                    return;
                }
            }

            //Set position
            transform.position = hit - (direction * impale);

            //Set rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);

            //Parent
            transform.SetParent(col.transform, true);
        }
    }

    public delegate void HitEvent(BaseUnit launcher, Faction faction, Collision2D collision);
}