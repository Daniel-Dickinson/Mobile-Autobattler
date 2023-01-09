using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class RangedUnit : BaseUnit
    {
        [Header("Ranged Attack")]
        [SerializeField] private Transform barrel;
        [SerializeField] private Projectile projectile;
        [SerializeField] private float projectileSpeed = 40.0f;
        [SerializeField] private float attackRangeMin = 2.8f;
        [SerializeField] private float attackRangeMax = 4.0f;
        [SerializeField] private float recovery = 0.1f;

        //Targeting
        private Vector3 attackPosition;

        //Action
        protected override float ActionRange(float distanceToTarget)
        {
            return Mathf.Clamp(distanceToTarget, attackRangeMin, attackRangeMax);
        }
        protected override void SetupAction(float deltaTime)
        {
            //Target required
            if (target == null) return;

            //Calculate distance to target
            float distance = Vector3.Distance(target.transform.position, transform.position);

            //Setup attack for next frame when available
            if (distance < (ActionRange(distance) + 0.05f) && projectile != null && perceiver.UnitVisible(target, true))
            {
                //Reduce targeting time
                targetTime -= deltaTime;

                //Attack when targeting complete
                if (targetTime <= 0)
                {
                    //Now attacking
                    state = UnitState.Actioning;

                    //Calculate direction
                    Vector3 direction = (target.transform.position - transform.position).normalized;

                    //Set attack position & time
                    attackPosition = target.transform.position + (direction * 0.15f);
                }
            }
        }
        protected override void Action(float deltaTime)
        {
            //Projectile required
            if (projectile == null) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            //Instantiate projectile
            Projectile proj = Instantiate(projectile, barrel.position, transform.rotation);

            //Add force
            proj.Launch(this, perceiver.Faction, (attackPosition - transform.position).normalized * projectileSpeed);

            //Now recovering
            Hold(recovery);
        }
    }
}