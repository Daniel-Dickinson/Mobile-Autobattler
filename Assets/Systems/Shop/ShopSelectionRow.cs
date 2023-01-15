using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class ShopSelectionRow : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;
        public ShopUnitPool pool;
        public ShopSelectionSlot slot;

        [Header("Tooltip")]
        public ShopTooltip tooltip;

        //Slots
        public int SlotCount
        {
            get { return slotCount; }
            set
            {
                if (value == slotCount) return;

                //Remove slots
                if (value < slotCount)
                {
                    while (value < slotCount)
                    {
                        Destroy(slots[slots.Count - 1]);
                        slots.RemoveAt(slots.Count - 1);
                        slotCount--;
                    }
                }

                //Add slots
                if (value > slotCount)
                {
                    while (value > slotCount)
                    {
                        AddSlot();
                        PopulateSlot(slots.Count - 1);
                        slotCount++;
                    }
                }
            }
        }
        private int slotCount = 5;

        //Slots
        private List<ShopSelectionSlot> slots;

        //Mono
        private void Awake()
        {
            SetupSlots();
        }
        private void Start()
        {
            PopulateSlots();
        }
        
        //Setup
        public void ResetSlots(int slotCount)
        {
            this.slotCount = slotCount;

            SetupSlots();
        }
        private void SetupSlots()
        {
            //Initialize or clear
            if (slots == null) slots = new List<ShopSelectionSlot>();
            else
            {
                for (int i = slots.Count -1; i >= 0; i--) DestroyImmediate(slots[i].gameObject);
                slots.Clear();
            }

            //Spawn new slots
            for (int i = 0; i < slotCount; i++)
            {
                AddSlot();
            }
        }
        private void AddSlot()
        {
            //Instaniate new slot
            ShopSelectionSlot newSlot = Instantiate(slot, transform);

            //Rename slot
            newSlot.name = slot.name;

            //Add slot
            slots.Add(newSlot);

            //Register to slot
            newSlot.OnSelected += SelectionChange;
        }

        //Populate
        public void PopulateSlots()
        {
            //Clear selection
            ClearSelection();

            //Slots required to populate
            if (slots == null) return;

            //Populate each slot
            for (int i = 0; i < slots.Count; i++)
            {
                PopulateSlot(i);
            }
        }
        private void PopulateSlot(int index)
        {
            //Don't populate locked slots
            if (slots[index].Locked) return;

            //Clear existing
            slots[index].DirectClear();

            //Get source
            ShopUnitIcon source = pool.GetRandomUnit(state.ShopLevel);

            //Create copy of icon
            ShopUnitIcon icon = Instantiate(source);
            icon.name = source.name;

            //Place in slot
            slots[index].PlaceUnit(icon);
        }

        //Selection
        public void ClearSelection()
        {
            if (slots != null)
            {
                //Deselect all slots
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].SetSelected(false);
                }
            }

            //Hide tooltip
            CloseToolTip();
        }
        private void SelectionChange(ShopUnitSlot slot)
        {
            //Deselect other slots
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != slot) slots[i].SetSelected(false);
            }

            //Show tool tip
            if (slot != null) ShowToolTip(slot);
            else CloseToolTip();
        }

        //Tooltip
        private void ShowToolTip(ShopUnitSlot slot)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.SetUnit(slot.Icon);
        }
        private void CloseToolTip()
        {
            tooltip.gameObject.SetActive(false);
        }
    }
}