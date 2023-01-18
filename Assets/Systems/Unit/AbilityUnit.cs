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
        public int HealIncrease
        {
            get { return healIncrease; }
            set { healIncrease = value; }
        }
        public int ChainIncrease
        {
            get { return chainIncrease; }
            set { chainIncrease = value; }
        }
        public int DamageIncrease
        {
            get { return damageIncrease; }
            set { damageIncrease = value; }
        }
        public float AOEMultiplier
        {
            get { return aoeMultiplier; }
            set { aoeMultiplier = value; }
        }
        public override void IncreaseRange(float amount)
        {
            base.IncreaseRange(amount);

            //Increase attackRangee range
            abilityRange *= (1.0f + amount);
        }

        //Buffing
        private int healIncrease = 0;
        private int chainIncrease = 0;
        private int damageIncrease = 0;
        private float aoeMultiplier = 1.0f;

        //Targeting
        private Vector3 abilityDirection;
        private Perceivable abilityTarget;        

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

                    //Request
                    Ability instance = PoolManager.RequestPoolable(ability, abilityTarget.transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection), abilityTarget.transform) as Ability;

                    //Apply buffs
                    instance.HealIncrease = healIncrease;
                    instance.DamageIncrease = damageIncrease;
                    instance.AOEMultiplier *= aoeMultiplier;

                    //Trigger & hold
                    instance.Trigger(perceiver, abilityTarget);
                    Hold(recovery);
                    break;

                case AbilityMode.Channeled:

                    //Request
                    channel = PoolManager.RequestPoolable(ability, abilityTarget.transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection)) as ChannelAbility;

                    //Attach to channel
                    channel.OnComplete += ClearChannel;
                    channel.OnInterrupt += ClearChannel;

                    //Apply buffs
                    channel.HealIncrease = healIncrease;
                    channel.DamageIncrease = damageIncrease;
                    channel.AOEMultiplier *= aoeMultiplier;

                    //Trigger & hold
                    channel.Trigger(perceiver, abilityTarget);
                    break;

                case AbilityMode.Chain:

                    //Request
                    ChainAbility chain = PoolManager.RequestPoolable(ability, transform.position, Quaternion.LookRotation(Vector3.forward, abilityDirection)) as ChainAbility;

                    //Apply buffs
                    chain.HealIncrease = healIncrease;
                    chain.DamageIncrease = damageIncrease;
                    chain.AOEMultiplier *= aoeMultiplier;

                    chain.ChainIncrease = chainIncrease;

                    //Trigger & hold
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