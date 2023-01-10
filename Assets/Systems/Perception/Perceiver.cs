using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Perception
{
    public class Perceiver : Perceivable
    {
        //Visibility
        public bool UnitVisible(Perceivable unit, bool testUnits = false)
        {
            //Calculate vector
            Vector3 vector = (unit.transform.position - transform.position);
            Vector3 direction = vector.normalized;
            float distance = vector.magnitude;

            //Calculate ray variables
            Vector3 rayStart = transform.position + (direction * radius);
            float rayDist = Mathf.Max(0, distance - radius - unit.radius);

            //If nearby don't need to raycast
            if (rayDist < 0.2f) return true;

            //Prep mask
            LayerMask mask = obstacleMask;
            if (testUnits) mask = unitMask | obstacleMask;

            //Raycast
            return !Physics2D.CircleCast(rayStart, 0.2f, direction, rayDist, mask);
        }

        //Targeting
        public Perceivable GetNearestTarget()
        {
            Perceivable target = null;
            float nearest = Mathf.Infinity;

            //Find nearest target
            for (int i = 0; i < perceivables.Count; i++)
            {
                if (perceivables[i] == null) continue;
                if (perceivables[i] == this) continue;
                if (!perceivables[i].IsHostileTowards(Faction)) continue;

                float distance = Vector3.Distance(perceivables[i].transform.position, transform.position);

                if (distance < nearest)
                {
                    target = perceivables[i];
                    nearest = distance;
                }
            }

            //Debug
            if (target != null && debugDirection) Debug.DrawLine(target.transform.position, transform.position, Color.red);

            return target;
        }
        public Perceivable GetNearestVisibleTarget()
        {
            Perceivable target = null;
            float nearest = Mathf.Infinity;

            //Find nearest target
            for (int i = 0; i < perceivables.Count; i++)
            {
                if (perceivables[i] == null) continue;
                if (perceivables[i] == this) continue;
                if (!perceivables[i].IsHostileTowards(Faction)) continue;

                //Check visibility
                if (!UnitVisible(perceivables[i], true)) continue;

                //Calculate distance
                float distance = Vector3.Distance(perceivables[i].transform.position, transform.position);

                //Select for nearest
                if (distance < nearest)
                {
                    target = perceivables[i];
                    nearest = distance;
                }
            }

            //Debug
            if (target != null && debugDirection) Debug.DrawLine(target.transform.position, transform.position, Color.red);

            //Fallback to nearest
            if (target == null) target = GetNearestTarget();

            //Return
            return target;
        }

        //Allies
        public Perceivable GetStrongestAlly()
        {
            Perceivable target = null;
            int strongest = 0;

            //Find nearest target
            for (int i = 0; i < perceivables.Count; i++)
            {
                if (perceivables[i] == null) continue;
                if (perceivables[i] == this) continue;
                if (perceivables[i].IsHostileTowards(Faction)) continue;

                //Grab unit
                BaseUnit unit = perceivables[i].GetComponent<BaseUnit>();
                int healthPool = unit.MaxHealth;

                //Check if closest
                if (healthPool > strongest)
                {
                    target = perceivables[i];
                    strongest = healthPool;
                }
            }

            //Debug
            if (target != null && debugDirection) Debug.DrawLine(target.transform.position, transform.position, Color.red);

            return target;
        }
        public Perceivable GetMostWoundedAlly()
        {
            Perceivable target = null;
            int mostHealthLost = 0;

            //Find nearest target
            for (int i = 0; i < perceivables.Count; i++)
            {
                if (perceivables[i] == null) continue;
                if (perceivables[i] == this) continue;
                if (perceivables[i].IsHostileTowards(Faction)) continue;

                //Grab unit
                BaseUnit unit = perceivables[i].GetComponent<BaseUnit>();
                int healthLost = unit.MaxHealth - unit.Health;

                //Ignore full health units
                if (healthLost == 0) continue;

                //Check if closest
                if (healthLost > mostHealthLost)
                {
                    target = perceivables[i];
                    mostHealthLost = healthLost;
                }
            }

            //Debug
            if (target != null && debugDirection) Debug.DrawLine(target.transform.position, transform.position, Color.red);

            //Fallback to get strongest ally
            if (target == null) target = GetStrongestAlly();

            //Return
            return target;
        }
        public Perceivable GetNearestWoundedAlly()
        {
            Perceivable target = null;
            float nearest = Mathf.Infinity;

            //Find nearest target
            for (int i = 0; i < perceivables.Count; i++)
            {
                if (perceivables[i] == null) continue;
                if (perceivables[i] == this) continue;
                if (perceivables[i].IsHostileTowards(Faction)) continue;

                //Grab unit
                BaseUnit unit = perceivables[i].GetComponent<BaseUnit>();
                int healthLost = unit.MaxHealth - unit.Health;

                //Ignore full health units
                if (healthLost == 0) continue;

                //Calculate distance to unit
                float distance = Vector3.Distance(perceivables[i].transform.position, transform.position);

                //Check if closest
                if (distance < nearest)
                {
                    target = perceivables[i];
                    nearest = distance;
                }
            }

            //Debug
            if (target != null && debugDirection) Debug.DrawLine(target.transform.position, transform.position, Color.red);

            //Fallback to get nearest ally
            if (target == null) target = GetStrongestAlly();

            //Return
            return target;
        }
    }
}