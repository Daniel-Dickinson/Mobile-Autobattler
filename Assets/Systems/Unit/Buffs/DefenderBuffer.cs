using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class DefenderBuffer : UnitBuffer
    {
        [SerializeField] private float health2 = 20;
        [SerializeField] private float health4 = 40;
        [SerializeField] private float health6 = 60;

        protected override void ApplyBuff(BaseUnit unit)
        {
            int count = state.GetCount(UnitClass.Defender);

            if (IsClass(unit, UnitClass.Defender)) ApplyBuff(unit, count);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            int count = state.GetCount(UnitClass.Defender);

            for (int i = 0; i < units.Count; i++)
            {
                if (IsClass(units[i], UnitClass.Defender)) ApplyBuff(units[i], count);
            }
        }

        private void ApplyBuff(BaseUnit unit, int count)
        {
            //Calculate buff
            float buff = GetBuffPercentile(count);
            int buffMultiplier = Mathf.CeilToInt(unit.MaxHealth * (buff / 100));

            //Increase health pool
            unit.RaiseMaxHealth(buffMultiplier);
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
                    return health2;
                case 4:
                case 5:
                    return health4;
                case > 5:
                    return health6;
            }
        }
    }
}