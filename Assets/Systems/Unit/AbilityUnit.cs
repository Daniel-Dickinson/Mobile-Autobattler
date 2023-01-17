using System.Collections;
using System.Collections.Generic;
using TwoBears.Perception;
using TwoBears.Pooling;
using UnityEngine;

namespace TwoBears.Unit
{
    public class AbilityUnit : BaseUnit
    {
        [Header("Ability")]
        [SerializeField] private Ability ability;
        [SerializeField] private AbilityMode mode;
        [SerializeField] private TargetingMode targeting;
        [SerializeField] private float abilityRange = 4.0f;
        [SerializeField] private float recovery = 0.1f;

        //Buff Access
        public int ChainIncrease
        {
            get { return chainIncrease; }
            set { chainIncrease = value; }
        }
        public float AOEMultiplier
        {
            get { return aoeMultiplier; }
            set { aoeMultiplier = value; }
        }

        //Buffing
        private int chainIncrease = 0;
        private float aoeMultiplier = 1.0f;

        //Targeting
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
                    abilityDirection = (abilityTarget.transform.position - transform.position).normalized;
                }
            }
        }
        protected override void Action(float deltaTime)
        {
            //Projectile required
            if (this.ability == null) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            switch (mode)
            {
                case AbilityMode.Attached:

                    Ability instance = PoolManager.RequestPoolable(ability, abilityTarget.transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection), abilityTarget.transform) as Ability;
                    instance.Trigger(perceiver, abilityTarget);
                    break;

                case AbilityMode.Chain:

                    ChainAbility chain = PoolManager.RequestPoolable(ability, transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection)) as ChainAbility;

                    chain.ChainIncrease = chainIncrease;
                    chain.DistanceMultiplier *= aoeMultiplier;

                    chain.Trigger(perceiver, abilityTarget);
                    break;
            }            

            //Play animation
            //if (anim != null) anim.Play("Attack");

            //Now recovering
            Hold(recovery);
        }
    }

    public enum TargetingMode { Caster, Healer, Necromancer }
    public enum AbilityMode { Attached, Chain }
}