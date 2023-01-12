using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class SummonerBuffer : UnitBuffer
    {
        [Header("Damage")]
        [SerializeField] private int damage2 = 1;
        [SerializeField] private int damage4 = 3;

        [Header("Health")]
        [SerializeField] private int health2 = 1;
        [SerializeField] private int health4 = 3;

        protected override void ApplyBuff(BaseUnit unit)
        {
            int count = state.GetCount(UnitClass.Summoner);
            if (IsClass(unit, UnitClass.Minion)) ApplyBuff(unit, count);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            int count = state.GetCount(UnitClass.Summoner);

            for (int i = 0; i < units.Count; i++)
            {
                if (IsClass(units[i], UnitClass.Minion)) ApplyBuff(units[i], count);
            }
        }

        private void ApplyBuff(BaseUnit unit, int count)
        {
            //Grab weapon
            Weapon weapon = unit.GetComponent<Weapon>();
            if (weapon == null) return;

            //Increase health
            unit.RaiseMaxHealth(GetHealthBuff(count));

            //Increase damage
            weapon.damage += GetDamageBuff(count);
        }
        private int GetDamageBuff(int count)
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
            }
        }
        private int GetHealthBuff(int count)
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
            }
        }
    }
}