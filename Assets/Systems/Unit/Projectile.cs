using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Stats")]
        public int damage = 1;

        [Header("Cleanup")]
        public float life = 5.0f;

        [Header("Layers")]
        public LayerMask units;
        public LayerMask armour;

        //State
        private Faction faction;
        private List<Rigidbody2D> ignore;

        //Components
        private Rigidbody2D rb;

        //Life
        private float lifeRemaining;

        //Mono
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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

            //Ignore the environment
            if (collision.rigidbody == null) return;

            //Ignore
            if (ignore.Contains(collision.rigidbody)) return;

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
    }
}