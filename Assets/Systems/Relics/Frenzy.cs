using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    public class Frenzy : Relic
    {
        [Header("Effect")]
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private float damageMultiplier = 0.5f;
        [SerializeField] private float attackSpeedMultiplier = 2.0f;

        //Core
        public override void ApplyBuff(RelicBuffer buffer, BaseUnit unit)
        {
            //Unit must match class
            if (!IsClass(unit, unitClass)) return;

            //Apply damage multiplier
            unit.DamageMultiplier *= damageMultiplier;

            //Apply attack speed
            unit.hesitanceMin *= (1 / attackSpeedMultiplier);
            unit.hesitanceMax *= (1 / attackSpeedMultiplier);
        }
        public override void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units)
        {
            for (int i = 0; i < units.Count; i++) ApplyBuff(buffer, units[i]);
        }
    }
}