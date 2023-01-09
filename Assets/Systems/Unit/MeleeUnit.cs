using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class MeleeUnit : BaseUnit
    {
        [Header("Melee Attack")]
        [SerializeField] private float chargeRange = 0.4f;
        [SerializeField] private float chargeSpeed = 2.5f;
        [SerializeField] private float strikeRange = 0.2f;
        [SerializeField] private float recovery = 1.0f;

        //Weapons
        protected Weapon weapon;

        //Attacks
        private Vector3 attackPosition;
        private float attackTime;

        //Mono
        protected override void OnEnable()
        {
            base.OnEnable();

            //Grab weapon
            weapon = GetComponent<Weapon>();
        }

        //Action
        protected override float ActionRange(float distanceToTarget)
        {
            return chargeRange;
        }
        protected override void SetupAction(float deltaTime)
        {
            //Target required
            if (target == null) return;

            //Calculate distance to target
            float distance = Vector3.Distance(target.transform.position, transform.position);

            //Setup attack for next frame when available
            if (distance < (chargeRange + 0.05f) && weapon != null && weapon.enabled && perceiver.UnitVisible(target))
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
                    attackTime = distance / chargeSpeed;
                }
            }
        }
        protected override void Action(float deltaTime)
        {
            //Weapon required
            if (weapon == null && weapon.enabled) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            //Calculate setup data
            float distance = Vector3.Distance(attackPosition, transform.position);
            Vector3 direction = (attackPosition - transform.position).normalized;
            Vector3 goalPosition = attackPosition - (direction * strikeRange);

            //Charge towards attack position
            rb.MovePosition(Vector3.MoveTowards(transform.position, goalPosition, chargeSpeed * deltaTime));

            //Rotate towards attack position
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            //Increment attack time (Fail safe in case attack position is unreachable)
            attackTime -= deltaTime;

            //Attack when within range
            if (attackTime <= 0 || distance < strikeRange + 0.05f)
            {
                //Strike
                weapon.Strike();

                //Now recovering
                Hold(recovery);
            }
        }

        //Death
        protected override void Kill()
        {
            //Base
            base.Kill();

            //Disable weapon
            if (weapon != null) weapon.enabled = false;
        }
    }
}