using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class ShopFormationRow : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;
        public ShopUnitPool unitPool;

        [Header("Setup")]
        public FormationRow row;
        public ShopFormationSlot slot;

        [Header("Limits")]
        public int maxSlots = 12;

        [Header("Elements")]
        public TextMeshProUGUI rowCost;

        //Upgrade
        private ShopButton upgradeButton;

        //Tracking
        private List<ShopFormationSlot> slots;
        private int rowExpandCost = 2;

        //Mono
        private void Awake()
        {
            //Grab upgrade button
            upgradeButton = GetComponentInChildren<ShopButton>();

            //Attach to player state
            state.OnSlotChange += Setup;
        }
        private void OnDestroy()
        {
            //Attach to player state
            state.OnSlotChange -= Setup;
        }
        private void Start()
        {
            //Initial setup
            Setup();
        }

        //Core
        private void Setup()
        {
            //Clear or initialize
            if (slots != null)
            {
                for (int i = slots.Count - 1; i >= 0; i--) DestroyImmediate(slots[i].gameObject);
                slots.Clear();
            }
            else slots = new List<ShopFormationSlot>();

            //Get slot count
            int slotCount = state.GetSlotCount(row);

            //Spawn slots
            for (int i = 0; i < slotCount; i++)
            {
                //Create slots
                ShopFormationSlot spawn = Instantiate(slot, transform);
                spawn.transform.SetSiblingIndex(i + 1);

                //Initialize slot
                spawn.Initialize(row, i, state, unitPool);

                //Track slot
                slots.Add(spawn);
            }

            //Set expand cost
            rowExpandCost = (slotCount * 2) - 2;
            upgradeButton.Cost = rowExpandCost;

            //Display expand cost
            rowCost.text = rowExpandCost.ToString();
        }

        //Expansion
        public void ExpandSlot()
        {
            //Coin required
            if (state.Gold < rowExpandCost) return;

            //Pay coin
            state.Gold -= rowExpandCost;

            //Expand
            state.IncreaseSlotCount(row);

            //Disable expansion if at limit
            if (state.GetSlotCount(row) >= maxSlots)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(transform.childCount - 2).gameObject.SetActive(false);
                transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
            }
        }
    }
}