using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(FormationSpawn))]
    public abstract class UnitBuffer : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected ClassCounter counter;

        private FormationSpawn spawner;

        //Mono
        private void Awake()
        {
            spawner = GetComponent<FormationSpawn>();
            spawner.OnSpawn += BuffAllUnits;
        }
        private void OnDestroy()
        {
            spawner.OnSpawn -= BuffAllUnits;
        }

        //Core
        private void BuffAllUnits()
        {
            ApplyBuffs(spawner.Spawns);
        }
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
}