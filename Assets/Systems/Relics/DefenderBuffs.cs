using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    public class DefenderBuffs : Relic
    {
        [Header("Effect")]
        [SerializeField] private UnitClass unitClass;

        //Core
        public override void ApplyBuff(RelicBuffer buffer, BaseUnit unit)
        {
            //Grab warrior buffer
            DefenderBuffer defender = buffer.GetComponent<DefenderBuffer>();

            //Apply
            ApplyBuff(defender, unit);
        }
        public override void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units)
        {
            //Grab warrior buffer
            DefenderBuffer defender = buffer.GetComponent<DefenderBuffer>();

            //Apply to all
            for (int i = 0; i < units.Count; i++) ApplyBuff(defender, units[i]);
        }

        //Optimized
        private void ApplyBuff(DefenderBuffer buffer, BaseUnit unit)
        {
            //Unit must match class
            if (!IsClass(unit, unitClass)) return;

            //Apply buffs
            buffer.ApplyBuffExternal(unit);
        }
    }
}