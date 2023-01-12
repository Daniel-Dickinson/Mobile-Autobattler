using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;
using TwoBears.Unit;

namespace TwoBears.Persistance
{
    [CreateAssetMenu(menuName = "TwoBears/PlayerState")]
    public class PlayerState : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField] private int gold = 12;
        [SerializeField] private int shards = 0;

        [Header("Formation")]
        [SerializeField] private Formation formation;
        [SerializeField] private UnitSelection selection;

        [Header("World")]
        [SerializeField] private int wave = 0;
        [SerializeField] private int shopLevel = 1;

        [Header("Slots")]
        [SerializeField] private int frontSlots = 2;
        [SerializeField] private int middleSlots = 3;
        [SerializeField] private int backSlots = 2;

        //Class Counts
        private List<int> countedUnits;
        private int warriors;
        private int defenders;
        private int rangers;
        private int healers;
        private int casters;
        private int merchants;
        private int summoners;

        //General
        public int Wave
        {
            get { return wave; }
            set
            {
                wave = value;
                OnWaveChange?.Invoke();
            }
        }
        public int Gold
        {
            get { return gold; }
            set
            {
                gold = value;
                OnGoldChange?.Invoke();
            }
        }
        public int Shards
        {
            get { return shards; }
            set
            {
                shards = value;
                OnShardChange?.Invoke();
            }
        }
        public int ShopLevel
        {
            get { return shopLevel; }
            set
            {
                if (shopLevel != value && shopLevel < 5)
                {
                    shopLevel = value;
                    OnShopLevelChange?.Invoke();
                }
            }
        }

        //Events
        public Action OnWaveChange;
        public Action OnGoldChange;
        public Action OnShardChange;
        public Action OnShopLevelChange;
        public Action OnFormationChange;
        public Action OnSlotChange;

        //Class Counts
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
                case UnitClass.Summoner:
                    return summoners;
            }
        }

        public bool UnitInFormation(int id)
        {
            if (countedUnits == null) return false;
            return countedUnits.Contains(id);
        }
        public void GetClasses(int id, out UnitClass primary, out UnitClass secondary, out UnitClass tertiary)
        {
            GameObject unit = selection.GetUnit(id, 0);
            BaseUnit baseUnit = unit.GetComponent<BaseUnit>();
            if (baseUnit == null) baseUnit = unit.GetComponentInChildren<BaseUnit>();

            primary = baseUnit.primary; 
            secondary = baseUnit.secondary; 
            tertiary = baseUnit.tertiary;
        }
        

        //Formation Slots
        public int GetSlotCount(FormationRow row)
        {
            switch (row)
            {
                default:
                case FormationRow.Front: 
                    return frontSlots;

                case FormationRow.Middle: 
                    return middleSlots;

                case FormationRow.Back: 
                    return backSlots;
            }
        }
        public void IncreaseSlotCount(FormationRow row)
        {
            //Increase slot count
            switch (row)
            {
                case FormationRow.Front:
                    
                    //Increase count
                    frontSlots++;

                    //Cache old units
                    FormationUnit[] oldFront = formation.front;

                    //Resize array
                    formation.front = new FormationUnit[frontSlots];

                    //Repopulate
                    for (int i = 0; i < oldFront.Length; i++) formation.front[i] = oldFront[i];
                    break;

                case FormationRow.Middle:

                    //Increase count
                    middleSlots++;

                    //Cache old units
                    FormationUnit[] oldMiddle = formation.middle;

                    //Resize array
                    formation.middle = new FormationUnit[middleSlots];

                    //Repopulate
                    for (int i = 0; i < oldMiddle.Length; i++) formation.middle[i] = oldMiddle[i];
                    break;

                case FormationRow.Back:

                    //Increase count
                    backSlots++;

                    //Cache old units
                    FormationUnit[] oldBack = formation.back;

                    //Resize array
                    formation.back = new FormationUnit[backSlots];

                    //Repopulate
                    for (int i = 0; i < oldBack.Length; i++) formation.back[i] = oldBack[i];
                    break;
            }

            //Slot change event
            OnSlotChange?.Invoke();
            OnFormationChange?.Invoke();
        }

        //Formation Access
        public int UnitCount
        {
            get
            {
                int total = 0;
                for (int i = 0; i < formation.front.Length; i++)
                {
                    if (formation.front[i] != null && formation.front[i].id >= 0) total++;
                }
                for (int i = 0; i < formation.middle.Length; i++)
                {
                    if (formation.middle[i] != null && formation.middle[i].id >= 0) total++;
                }
                for (int i = 0; i < formation.back.Length; i++)
                {
                    if (formation.back[i] != null && formation.back[i].id >= 0) total++;
                }
                return total;
            }
        }
        public FormationUnit GetFormationUnit(FormationRow row, int position)
        {
            switch (row)
            {
                default:
                case FormationRow.Front:
                    return formation.front[position];

                case FormationRow.Middle:
                    return formation.middle[position];

                case FormationRow.Back:
                    return formation.back[position];
            }
        }
        public void SetFormationUnit(FormationRow row, int position, FormationUnit unit)
        {
            switch (row)
            {
                case FormationRow.Front:
                    formation.front[position] = unit;
                    break;

                case FormationRow.Middle:
                    formation.middle[position] = unit;
                    break;

                case FormationRow.Back:
                    formation.back[position] = unit;
                    break;
            }

            //Update unit counts
            UpdateCount();

            //Formation change event
            OnFormationChange?.Invoke();
        }

        //Class Counting
        private void UpdateCount()
        {
            ClearCounts();

            CountArray(formation.front);
            CountArray(formation.middle);
            CountArray(formation.back);
        }
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
            summoners = 0;
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

            //Get child unit if swarm
            if (baseUnit == null) baseUnit = goUnit.GetComponentInChildren<BaseUnit>();

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
                case UnitClass.Summoner:
                    summoners++;
                    break;
            }
        }

        //Serialization
        public void Save()
        {

        }
        public void Load()
        {

        }
        
        //Restart
        public void Restart()
        {
            //Reset shop level
            shopLevel = 1;

            //Reset slots
            frontSlots = 3;
            middleSlots = 4;
            backSlots = 3;

            //Clear formation
            formation.front = new FormationUnit[frontSlots];
            formation.middle = new FormationUnit[middleSlots];
            formation.back = new FormationUnit[backSlots];

            //Clear unit counts;
            ClearCounts();

            //Reset wave
            wave = 0;

            //Reset gold
            gold = 15;

            //Trigger Events
            OnGoldChange?.Invoke();
            OnSlotChange?.Invoke();
            OnFormationChange?.Invoke();
        }
    }

    public enum FormationRow { Front, Middle, Back }
}