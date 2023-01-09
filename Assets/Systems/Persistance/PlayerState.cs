using System;
using System.Collections;
using System.Collections.Generic;
using TwoBears.Waves;
using UnityEngine;

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

        [Header("World")]
        [SerializeField] private int wave = 0;
        [SerializeField] private int shopLevel = 1;

        [Header("Slots")]
        [SerializeField] private int frontSlots = 2;
        [SerializeField] private int middleSlots = 3;
        [SerializeField] private int backSlots = 2;

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

            //Formation change event
            OnFormationChange?.Invoke();
        }

        //Events
        public Action OnWaveChange;
        public Action OnGoldChange;
        public Action OnShardChange;
        public Action OnShopLevelChange;
        public Action OnFormationChange;
        public Action OnSlotChange;

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