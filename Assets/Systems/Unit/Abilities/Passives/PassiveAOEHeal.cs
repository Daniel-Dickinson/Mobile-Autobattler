using System.Collections;
using System.Collections.Generic;
using TwoBears.Perception;
using UnityEngine;

namespace TwoBears.Unit
{
    public class PassiveAOEHeal : PassiveAOE
    {
        [Header("Effect")]
        public int heal = 1;

        protected override bool IsValidTarget(BaseUnit self, BaseUnit unit)
        {
            return self.GetComponent<Perceivable>().Faction == unit.GetComponent<Perceivable>().Faction;
        }
        protected override void ApplyEffect(BaseUnit unit)
        {
            unit.RestoreHealth(heal);
        }
    }
}