using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;
using TwoBears.Persistance;

namespace TwoBears.Unit
{
    public class ClassCounter : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerState state;
        [SerializeField] private Formation formation;
        [SerializeField] private UnitSelection selection;

        //Mono
        private void Awake()
        {
            state.OnFormationChange += UpdateCount;
        }
        private void OnDestroy()
        {
            state.OnFormationChange -= UpdateCount;
        }

        //Access
        public Action OnCount;

        //Counted
        private List<int> countedUnits;

        //Counts
        private int warriors;
        private int defenders;
        private int rangers;
        private int healers;
        private int casters;
        private int merchants;

        //Core
        private void UpdateCount()
        {
            ClearCounts();
            CountArray(formation.front);
            CountArray(formation.middle);
            CountArray(formation.back);

            OnCount?.Invoke();
        }
        public int GetCount(UnitClass unitClass)
        {
            switch (unitClass)
            {
                default:
                case UnitClass.None:
                    return 0;
                case UnitClass.Warrior:
                    return warriors;
                case UnitClass.Defender:
                    return defenders;
                case UnitClass.Ranger:
                    return rangers;
                case UnitClass.Healer:
                    return healers;
                case UnitClass.Caster:
                    return casters;
                case UnitClass.Merchant:
                    return merchants;
            }
        }

        //Counting
        private void ClearCounts()
        {
            if (countedUnits == null) countedUnits = new List<int>();
            else countedUnits.Clear();

            warriors = 0;
            defenders = 0;
            rangers = 0;
            healers = 0;
            casters = 0;
            merchants = 0;
        }
        private void CountArray(FormationUnit[] units)
        {
            if (units == null) return;
            for (int i = 0; i < units.Length; i++) CountUnit(units[i]);
        }
        private void CountUnit(FormationUnit unit)
        {
            if (unit == null || unit.id < 0) return;
            if (countedUnits.Contains(unit.id)) return;

            //Grab unit
            GameObject goUnit = selection.GetUnit(unit.id, 0);
            BaseUnit baseUnit = goUnit.GetComponent<BaseUnit>();

            //Count all classes
            AddToCount(baseUnit.primary);
            AddToCount(baseUnit.secondary);
            AddToCount(baseUnit.tertiary);

            //Only count each unit type once
            countedUnits.Add(unit.id);
        }
        private void AddToCount(UnitClass unitClass)
        {
            switch (unitClass)
            {
                case UnitClass.Warrior:
                    warriors++;
                    break;

                case UnitClass.Defender:
                    defenders++;
                    break;

                case UnitClass.Ranger:
                    rangers++;
                    break;

                case UnitClass.Healer:
                    healers++;
                    break;

                case UnitClass.Caster:
                    casters++;
                    break;

                case UnitClass.Merchant:
                    merchants++;
                    break;
            }
        }
    }
}