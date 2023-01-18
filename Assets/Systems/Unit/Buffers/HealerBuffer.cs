using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class HealerBuffer : UnitBuffer
    {
        [Header("Effect")]
        [SerializeField] private int healing2 = 1;
        [SerializeField] private int healing4 = 2;

        //Access
        public void ApplyBuffExternal(BaseUnit unit)
        {
            //Grab internal count
            int count = state.GetCount(UnitClass.Healer);

            //Increase healing
            ApplyBuff(unit, GetBuff(count));
        }

        //Core
        protected override void ApplyBuff(BaseUnit unit)
        {
            int count = state.GetCount(UnitClass.Healer);

            if (IsClass(unit, UnitClass.Healer)) ApplyBuff(unit, count);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            int count = state.GetCount(UnitClass.Healer);

            for (int i = 0; i < units.Count; i++)
            {
                if (IsClass(units[i], UnitClass.Healer)) ApplyBuff(units[i], count);
            }
        }

        private void ApplyBuff(BaseUnit unit, int count)
        {
            //Grab & buff ability unit
            AbilityUnit abilityUnit = unit as AbilityUnit;
            if (abilityUnit != null) abilityUnit.HealIncrease += GetBuff(count);

            //Grab & buff child passives
            PassiveAbility[] passives = unit.GetComponentsInChildren<PassiveAbility>();
            for (int i = 0; i < passives.Length; i++) passives[i].HealIncrease += GetBuff(count);
        }
        private int GetBuff(int count)
        {
            switch (count)
            {
                default:
                case < 2:
                    return 0;
                case 2:
                case 3:
                    return healing2;
                case >3:
                    return healing4;
            }
        }
    }
}