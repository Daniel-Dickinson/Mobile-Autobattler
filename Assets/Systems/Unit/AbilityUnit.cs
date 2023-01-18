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

        //Channel ability
        private ChannelAbility channel;

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
                    movementTarget = actionTarget;
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
            if (channel == null && distance < abilityRange && ability != null && ability.IsTargetValid(perceiver, actionTarget))
            {
                //Reduce targeting time
                targetTime -= deltaTime;

                //Attack when targeting complete
                if (targetTime <= 0)
                {
                    //Now actioning
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
            if (ability == null) return;

            //Must still be actioning
            if (state != UnitState.Actioning) return;

            //Ignore if already channeling
            if (channel != null) return;

            switch (mode)
            {
                case AbilityMode.Instant:

                    Ability instance = PoolManager.RequestPoolable(ability, abilityTarget.transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection), abilityTarget.transform) as Ability;
                    instance.Trigger(perceiver, abilityTarget);
                    Hold(recovery);
                    break;

                case AbilityMode.Channeled:

                    channel = PoolManager.RequestPoolable(ability, abilityTarget.transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection)) as ChannelAbility;

                    channel.OnComplete += ClearChannel;
                    channel.OnInterrupt += ClearChannel;

                    channel.Trigger(perceiver, abilityTarget);
                    break;

                case AbilityMode.Chain:

                    ChainAbility chain = PoolManager.RequestPoolable(ability, transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection)) as ChainAbility;

                    chain.ChainIncrease = chainIncrease;
                    chain.DistanceMultiplier *= aoeMultiplier;

                    chain.Trigger(perceiver, abilityTarget);
                    Hold(recovery);
                    break;
            }
        }

        //Channeling
        public override void KnockBack(Vector2 knockback)
        {
            //Knockback
            base.KnockBack(knockback);

            //Interupt channels
            if (channel != null) channel.Interrupt();
        }
        public void ClearChannel()
        {
            //Detach
            channel.OnComplete -= ClearChannel;
            channel.OnInterrupt -= ClearChannel;

            //Now recovering
            Hold(recovery);

            //Clear
            channel = null;
        }
    }

    public enum TargetingMode { Caster, Healer, Necromancer }
    public enum AbilityMode { Instant, Channeled, Chain }
}