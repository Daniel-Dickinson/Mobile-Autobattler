using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;
using TwoBears.Persistance;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(FormationSpawn))]
    public abstract class UnitBuffer : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected PlayerState state;
        [SerializeField] protected BuffTiming timing;

        private FormationSpawn spawner;

        //Mono
        private void Awake()
        {
            spawner = GetComponent<FormationSpawn>();

            if (timing == BuffTiming.Primary)
            {
                spawner.OnSpawn += BuffAllUnits;
                spawner.OnSummon += ApplyBuff;
            }
            else
            {
                spawner.PostSpawn += BuffAllUnits;
                spawner.PostSummon += ApplyBuff;
            }
        }
        private void OnDestroy()
        {
            if (timing == BuffTiming.Primary)
            {
                spawner.OnSpawn -= BuffAllUnits;
                spawner.OnSummon -= ApplyBuff;
            }
            else
            {
                spawner.PostSpawn -= BuffAllUnits;
                spawner.PostSummon -= ApplyBuff;
            }
        }

        //Core
        private void BuffAllUnits()
        {
            ApplyBuffs(spawner.Spawns);
        }

        //Apply
        protected abstract void ApplyBuff(BaseUnit unit);
        protected abstract void ApplyBuffs(List<BaseUnit> units);

        //Utility
        protected bool IsClass(BaseUnit unit, UnitClass unitClass)
        {
            if (unit.primary == unitClass) return true;
            if (unit.secondary == unitClass) return true;
            if (unit.tertiary == unitClass) return true;

            return false;
        }
    }

    public enum BuffTiming { Primary, Secondary };
}