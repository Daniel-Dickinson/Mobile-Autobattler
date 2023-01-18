using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    public class Explosion : MonoBehaviour
    {
        [Header("Metrics")]
        public float radius = 0.5f;
        public float knockback = 5.0f;
        public int damage = 2;

        [Header("Effects")]
        [SerializeField] private ParticleSystem effect;

        //Access
        public void Trigger()
        {
            //Grab perceivables
            List<Perceivable> targets = Perceivable.Perceivables;

            //Knockback all units in radius
            for (int i = 0; i < targets.Count; i++)
            {
                //Check faction
                if (targets[i] == null) continue;

                //Calculate distance
                Vector3 vector = targets[i].transform.position - transform.position;
                float distance = vector.magnitude;

                //Distance check
                if (distance > radius) continue;

                //Grab base unit
                BaseUnit unit = targets[i].GetComponent<BaseUnit>();

                //Knock units back
                if (knockback > 0) unit.KnockBack(vector.normalized * knockback);

                //Apply damage
                if (damage > 0) unit.RemoveHealth(damage);
            }

            //Play particles
            if (effect != null) effect.Play();
        }
        public void TriggerAgainst(Faction faction)
        {
            //Grab perceivables
            List<Perceivable> targets = Perceivable.Perceivables;

            //Knockback all units of faction
            for (int i = 0; i < targets.Count; i++)
            {
                //Check faction
                if (targets[i] == null) continue;
                if (targets[i].Faction != faction) continue;

                //Calculate distance
                Vector3 vector = targets[i].transform.position - transform.position;
                float distance = vector.magnitude;

                //Distance check
                if (distance > radius) continue;

                //Grab base unit
                BaseUnit unit = targets[i].GetComponent<BaseUnit>();

                //Knock units back
                if (knockback > 0) unit.KnockBack(vector.normalized * knockback);

                //Apply damage
                if (damage > 0) unit.RemoveHealth(damage);
            }

            //Play particles
            if (effect != null) effect.Play();
        }
    }
}