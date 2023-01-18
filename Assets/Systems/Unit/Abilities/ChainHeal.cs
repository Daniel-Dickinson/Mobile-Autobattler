using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    public class ChainHeal : ChainAbility
    {
        [Header("Effect")]
        [SerializeField] private int heal = 1;

        //Targeting
        public override bool IsTargetValid(Perceivable self, Perceivable target)
        {
            //Target must be of same faction
            if (target.Faction != self.Faction) return false;

            //Grab unit
            BaseUnit unit = target.GetComponent<BaseUnit>();

            //Target must be alive && require healing
            if (unit == null || unit.Health == 0 || unit.Health == unit.MaxHealth) return false;

            //Target is valid
            return true;
        }
        protected override bool IsValidChainTarget(BaseUnit unit)
        {
            //Target must be alive && require healing
            if (unit == null || unit.Health == 0 || unit.Health == unit.MaxHealth) return false;

            //Success
            return true;
        }

        //Effect
        protected override void TriggerChainEffect(BaseUnit unit, Vector3 direction)
        {
            //Remove health
            unit.RestoreHealth(heal);
        }
    }
}