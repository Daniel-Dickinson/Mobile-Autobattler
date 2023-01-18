using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class CasterBuffer : UnitBuffer
    {
        [Header("Effect")]
        [SerializeField] private float aoe2 = 20;
        [SerializeField] private float aoe4 = 40;
        [SerializeField] private float aoe6 = 60;

        //Access
        public void ApplyBuffExternal(BaseUnit unit)
        {
            int count = state.GetCount(UnitClass.Caster);
            ApplyBuff(unit, count);
        }

        //Core
        protected override void ApplyBuff(BaseUnit unit)
        {
            int count = state.GetCount(UnitClass.Caster);

            if (IsClass(unit, UnitClass.Caster)) ApplyBuff(unit, count);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            int count = state.GetCount(UnitClass.Caster);

            for (int i = 0; i < units.Count; i++)
            {
                if (IsClass(units[i], UnitClass.Caster)) ApplyBuff(units[i], count);
            }
        }

        private void ApplyBuff(BaseUnit unit, int count)
        {
            //Calculate buff
            float buff = GetBuffPercentile(count);
            float buffMultiplier = 1 + Mathf.Clamp01(buff / 100.0f);

            //Grab & buff ability unit
            AbilityUnit abilityUnit = unit as AbilityUnit;
            if (abilityUnit != null) abilityUnit.AOEMultiplier *= buffMultiplier;

            //Grab & buff child passives
            PassiveAbility[] passives = unit.GetComponentsInChildren<PassiveAbility>();
            for (int i = 0; i < passives.Length; i++) passives[i].AOEMultiplier *= buffMultiplier;
        }
        private float GetBuffPercentile(int count)
        {
            switch (count)
            {
                default:
                case < 2:
                    return 0;
                case 2:
                case 3:
                    return aoe2;
                case 4:
                case 5:
                    return aoe4;
                case > 5:
                    return aoe6;
            }
        }
    }
}