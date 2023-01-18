using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    public class ChainDamage : ChainAbility
    {
        [Header("Effect")]
        [SerializeField] private int damage = 1;
        [SerializeField] private int knockback = 1;

        //Targeting
        public override bool IsTargetValid(Perceivable self, Perceivable target)
        {
            //Target must be of opposite faction
            if (target.Faction == self.Faction) return false;

            //Grab unit
            BaseUnit unit = target.GetComponent<BaseUnit>();

            //Target must be alive
            if (unit == null || unit.Health == 0) return false;

            //Target is valid
            return true;
        }

        //Effect
        protected override void TriggerChainEffect(BaseUnit unit, Vector3 direction)
        {
            //Remove health
            unit.RemoveHealth(damage + damageIncrease);

            //Knockback slightly
            unit.KnockBack(direction * knockback);
        }
    }
}