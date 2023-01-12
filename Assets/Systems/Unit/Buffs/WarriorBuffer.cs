using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class WarriorBuffer : UnitBuffer
    {
        [SerializeField] private int damage2 = 1;
        [SerializeField] private int damage4 = 3;
        [SerializeField] private int damage6 = 6;

        [SerializeField] private float knockback2 = 20;
        [SerializeField] private float knockback4 = 40;
        [SerializeField] private float knockback6 = 60;

        protected override void ApplyBuff(BaseUnit unit)
        {
            int count = state.GetCount(UnitClass.Warrior);

            if (IsClass(unit, UnitClass.Warrior)) ApplyBuff(unit, count);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            int count = state.GetCount(UnitClass.Warrior);

            for (int i = 0; i < units.Count; i++)
            {
                if (IsClass(units[i], UnitClass.Warrior)) ApplyBuff(units[i], count);
            }
        }

        private void ApplyBuff(BaseUnit unit, int count)
        {
            //Grab weapon
            Weapon weapon = unit.GetComponent<Weapon>();
            if (weapon == null) return;

            //Calculate knockback buff
            float buff = GetKnockbackPercentile(count);
            float buffMultiplier = 1 + (buff / 100.0f);

            //Increase knockback
            weapon.knockback *= buffMultiplier;

            //Increase damage
            weapon.damage += GetDamage(count);
        }

        private int GetDamage(int count)
        {
            switch (count)
            {
                default:
                case < 2:
                    return 0;
                case 2:
                case 3:
                    return damage2;
                case 4:
                case 5:
                    return damage4;
                case > 5:
                    return damage6;
            }
        }
        private float GetKnockbackPercentile(int count)
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