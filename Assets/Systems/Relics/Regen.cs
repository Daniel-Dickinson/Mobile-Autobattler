using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    public class Regen : Relic
    {
        [Header("Effect")]
        [SerializeField] private UnitClass unitClass;
        [SerializeField] private int amount = 3;
        [SerializeField] private float tickSpeed = 0.33f;
        
        //Core
        public override void ApplyBuff(RelicBuffer buffer, BaseUnit unit)
        {
            //Unit must match class
            if (!IsClass(unit, unitClass)) return;

            //Add passive regen to unit
            PassiveRegen behaviour = unit.gameObject.AddComponent<PassiveRegen>();

            //Setup behaviour
            behaviour.amount = amount;
            behaviour.tickSpeed = tickSpeed;
        }
        public override void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units)
        {
            for (int i = 0; i < units.Count; i++) ApplyBuff(buffer, units[i]);
        }
    }
}