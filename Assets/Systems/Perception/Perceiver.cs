using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Perception
{
    public class Perceiver : Perceivable
    {
        [Header("Targeting")]
        public TargetingMode targeting;

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
        public Perceivable GetTarget()
        {
            switch (targeting)
            {
                default:
                case TargetingMode.Melee:
                    return GetNearestTarget();

                case TargetingMode.Ranged:
                    Perceivable target = GetNearestVisibleTarget();
                    if (target == null) target = GetNearestTarget();
                    return target;

                case TargetingMode.Healer:
                    return GetNearestWoundedAlly();
            }
        }

        private Perceivable GetNearestTarget()
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
        private Perceivable GetNearestVisibleTarget()
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

            return target;
        }
        private Perceivable GetNearestWoundedAlly()
        {
            Perceivable target = null;
            float nearest = Mathf.Infinity;

            //Find nearest target
            for (int i = 0; i < perceivables.Count; i++)
            {
                if (perceivables[i] == null) continue;
                if (perceivables[i] == this) continue;
                if (perceivables[i].IsHostileTowards(Faction)) continue;

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
    }
    public enum TargetingMode { Melee, Ranged, Healer }
}