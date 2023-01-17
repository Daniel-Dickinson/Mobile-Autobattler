using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    public class ChainIncrease : Relic
    {
        [Header("Effect")]
        [SerializeField] private int amount = 2;

        //Core
        public override void ApplyBuff(RelicBuffer buffer, BaseUnit unit)
        {
            //Unit must match class
            if (!IsClass(unit, UnitClass.Caster)) return;

            //Grab unit as ability unit
            AbilityUnit abilityUnit = unit as AbilityUnit;
            if (abilityUnit == null) return;

            //Increase movement & attack range
            abilityUnit.ChainIncrease += amount;
        }
        public override void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units)
        {
            for (int i = 0; i < units.Count; i++) ApplyBuff(buffer, units[i]);
        }
    }
}