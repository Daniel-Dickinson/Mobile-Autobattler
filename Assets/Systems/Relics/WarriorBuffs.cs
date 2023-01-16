using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    public class WarriorBuffs : Relic
    {
        [Header("Effect")]
        [SerializeField] private UnitClass unitClass;

        //Core
        public override void ApplyBuff(RelicBuffer buffer, BaseUnit unit)
        {
            //Grab warrior buffer
            WarriorBuffer warrior = buffer.GetComponent<WarriorBuffer>();

            //Apply
            ApplyBuff(warrior, unit);
        }
        public override void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units)
        {
            //Grab warrior buffer
            WarriorBuffer warrior = buffer.GetComponent<WarriorBuffer>();

            //Apply to all
            for (int i = 0; i < units.Count; i++) ApplyBuff(warrior, units[i]);
        }

        //Optimized
        private void ApplyBuff(WarriorBuffer buffer, BaseUnit unit)
        {
            //Unit must match class
            if (!IsClass(unit, unitClass)) return;

            //Apply buffs
            buffer.ApplyBuffExternal(unit);
        }
    }
}
