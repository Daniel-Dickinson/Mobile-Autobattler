using System.Collections;
using System.Collections.Generic;
using TwoBears.Perception;
using UnityEngine;

namespace TwoBears.Unit
{
    public class AbilityUnit : BaseUnit
    {
        [Header("Ability")]
        [SerializeField] private Ability ability;
        [SerializeField] private TargetingMode targeting;
        [SerializeField] private bool attachToTarget = false;
        [SerializeField] private float abilityRange = 4.0f;
        [SerializeField] private float recovery = 0.1f;

        //Targeting
        private Vector3 abilityPosition;
        private Vector3 abilityDirection;
        private Perceivable abilityTarget;

        //Buffing
        public override void IncreaseRange(float amount)
        {
            base.IncreaseRange(amount);

            //Increase attackRangee range
            abilityRange *= (1.0f + amount);
        }

        //Ability
        protected override void Targeting()
        {
            switch (targeting)
            {
                case TargetingMode.Caster:
                    actionTarget = perceiver.GetNearestVisibleTarget();
                    movementTarget = actionTarget;
                    break;
                case TargetingMode.Healer:
                    actionTarget = perceiver.GetNearestWoundedAlly();
                    movementTarget = perceiver.GetMostWoundedAlly();
                    break;
                case TargetingMode.Necromancer:
                    actionTarget = perceiver.GetNearestVisibleCorpse();
                    movementTarget = actionTarget;
                    break;
            }
            
        }

        protected override void SetupAction(float deltaTime)
        {
            //Target required
            if (actionTarget == null) return;
            
            //Calculate distance to target
            float distance = Vector3.Distance(actionTarget.transform.position, transform.position);

            //Setup attack for next frame when available
            if (distance < abilityRange && ability != null && ability.IsTargetValid(perceiver, actionTarget))
            {
                //Reduce targeting time
                targetTime -= deltaTime;

                //Attack when targeting complete
                if (targetTime <= 0)
                {
                    //Now attacking
                    state = UnitState.Actioning;

                    //Set ability target
                    abilityTarget = actionTarget;

                    //Calculate direction
                    abilityPosition = abilityTarget.transform.position;
                    abilityDirection = (abilityTarget.transform.position - transform.position).normalized;
                }
            }
        }
        protected override void Action(float deltaTime)
        {
            //Projectile required
            if (ability == null) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            //Declare ability
            Ability proj = null;

            //Instantiate projectile
            if (attachToTarget) proj = Instantiate(ability, abilityTarget.transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection), abilityTarget.transform);
            else proj = Instantiate(ability, abilityPosition, Quaternion.LookRotation(Vector3.forward, abilityDirection));

            //Tigger ability
            proj.Trigger(perceiver, abilityTarget);

            //Play animation
            //if (anim != null) anim.Play("Attack");

            //Now recovering
            Hold(recovery);
        }
    }

    public enum TargetingMode { Caster, Healer, Necromancer }
}