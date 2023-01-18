using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class DefenderBuffer : UnitBuffer
    {
        [Header("Effect")]
        [SerializeField] private int health2 = 2;
        [SerializeField] private int health4 = 4;
        [SerializeField] private int health6 = 6;

        //Access
        public void ApplyBuffExternal(BaseUnit unit)
        {
            //Grab internal count
            int count = state.GetCount(UnitClass.Defender);

            //Increase health pool
            unit.RaiseMaxHealth(GetBuff(count));
        }

        //Core
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
            //Increase health pool
            unit.RaiseMaxHealth(GetBuff(count));
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