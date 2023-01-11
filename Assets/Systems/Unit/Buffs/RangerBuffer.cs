using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class RangerBuffer : UnitBuffer
    {
        [SerializeField] private float attSpd2 = 10;
        [SerializeField] private float attSpd4 = 25;
        [SerializeField] private float attSpd6 = 50;

        protected override void ApplyBuff(BaseUnit unit)
        {
            int count = counter.GetCount(UnitClass.Ranger);

            if (IsClass(unit, UnitClass.Ranger)) ApplyBuff(unit, count);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            int count = counter.GetCount(UnitClass.Ranger);

            for (int i = 0; i < units.Count; i++)
            {
                if (IsClass(units[i], UnitClass.Ranger)) ApplyBuff(units[i], count);
            }
        }

        private void ApplyBuff(BaseUnit unit, int count)
        {
            //Calculate buff
            float buff = GetBuffPercentile(count);
            float buffMultiplier = 1 / (1 + (buff / 100.0f));

            //Increase attack speed
            unit.hesitanceMin *= buffMultiplier;
            unit.hesitanceMax *= buffMultiplier;
        }
        private float GetBuffPercentile(int count)
        {
            switch (count)
            {
                default:
                case <2:
                    return 0;
                case 2:
                case 3:
                    return attSpd2;
                case 4:
                case 5:
                    return attSpd4;
                case >5:
                    return attSpd6;
            }
        }
    }
}