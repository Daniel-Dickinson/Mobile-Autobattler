using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioSource))]
    public class Projectile : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int damage = 1;

        [Header("Impale")]
        [SerializeField] private float impale = 0.0f;

        [Header("Cleanup")]
        [SerializeField] private float life = 5.0f;

        [Header("Layers")]
        [SerializeField] private LayerMask units;
        [SerializeField] private LayerMask armour;

        //State
        private Faction faction;
        private List<Rigidbody2D> ignore;
        private BaseUnit launcher;

        //Components
        private Rigidbody2D rb;
        private AudioSource audioSource;

        //Life
        private float lifeRemaining;

        //Mono
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();

            lifeRemaining = life;
        }
        private void FixedUpdate()
        {
            //Reduce life
            lifeRemaining -= Time.fixedDeltaTime;

            //Destroy self
            if (lifeRemaining <= 0) Destroy(gameObject);
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
        }

        //Physics
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Reduce life
            lifeRemaining = Mathf.Min(0.1f, lifeRemaining);

            //Impale
            if (impale > 0) Impale(collision.collider, collision.contacts[0].point);

            //Ignore the environment
            if (collision.rigidbody == null) return;

            //Ignore
            if (ignore.Contains(collision.rigidbody)) return;

            //Play hit
            audioSource.Play();

            //Deactivate on armour hits
            if (armour == (armour | (1 << collision.rigidbody.gameObject.layer)))
            {
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

                //Ignore
                ignore.Add(collision.rigidbody);

                return;
            }
        }

        private void Impale(Collider2D col, Vector3 hit)
        {
            //Calculate direction from launcher
            Vector3 direction = (launcher.transform.position - hit).normalized;

            //Disable physics
            rb.simulated = false;
            GetComponent<Collider2D>().enabled = false;

            //Extend life
            lifeRemaining = life * 5.0f;

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
}