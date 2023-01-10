using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class WarriorBuffer : UnitBuffer
    {
        [SerializeField] private float knockback2 = 30;
        [SerializeField] private float knockback4 = 60;
        [SerializeField] private float knockback6 = 100;

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
            //Grab weapon
            Weapon weapon = unit.GetComponent<Weapon>();
            if (weapon == null) return;

            //Calculate buff
            float buff = GetBuffPercentile(count);
            float buffMultiplier = 1 + (buff / 100.0f);

            //Increase attack speed
            weapon.knockback *= buffMultiplier;
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
                    return knockback2;
                case 4:
                case 5:
                    return knockback4;
                case > 5:
                    return knockback6;
            }
        }
    }
}