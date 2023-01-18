using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class WarriorBuffer : UnitBuffer
    {
        [Header("Damage")]
        [SerializeField] private int damage2 = 1;
        [SerializeField] private int damage4 = 3;
        [SerializeField] private int damage6 = 6;

        [Header("Move Speed")]
        [SerializeField] private float moveSpeed2 = 1.0f;
        [SerializeField] private float moveSpeed4 = 1.5f;
        [SerializeField] private float moveSpeed6 = 2.0f;

        //Access
        public void ApplyBuffExternal(BaseUnit unit)
        {
            //Grab internaal count
            int count = state.GetCount(UnitClass.Warrior);
            ApplyBuff(unit, count);
        }

        //Core
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
            //Increase movespeed
            unit.MoveSpeedIncrease += GetMoveSpeedIncrease(count);

            //Grab weapon
            Weapon weapon = unit.GetComponent<Weapon>();
            if (weapon == null) return;

            //Increase damage
            weapon.BaseDamage += GetDamage(count);
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
        private float GetMoveSpeedIncrease(int count)
        {
            switch (count)
            {
                default:
                case < 2:
                    return 0;
                case 2:
                case 3:
                    return moveSpeed2;
                case 4:
                case 5:
                    return moveSpeed4;
                case > 5:
                    return moveSpeed6;
            }
        }
    }
}