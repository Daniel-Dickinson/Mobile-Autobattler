using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    public class Lifesteal : Relic
    {
        [Header("Effect")]
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private float lifesteal = 0.5f;

        //Core
        public override void ApplyBuff(RelicBuffer buffer, BaseUnit unit)
        {
            //Unit must match class
            if (!IsClass(unit, unitClass)) return;

            //Grab weapon
            Weapon weapon = unit.GetComponent<Weapon>();

            //Apply lifesteal
            if (weapon != null) weapon.Lifesteal += lifesteal;
        }
        public override void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units)
        {
            for (int i = 0; i < units.Count; i++) ApplyBuff(buffer, units[i]);
        }
    }
}