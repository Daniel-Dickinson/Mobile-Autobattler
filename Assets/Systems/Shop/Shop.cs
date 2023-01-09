using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;
using TMPro;

namespace TwoBears.Shop
{
    public class Shop : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;

        [Header("Selection")]
        public ShopSelectionRow selection;

        [Header("SellSlot")]
        public ShopSellSlot sellSlot;

        [Header("Title")]
        public TextMeshProUGUI title;

        [Header("Upgrade Button")]
        public ShopButton upgradeButton;
        public TextMeshProUGUI upgradeCost;

        [Header("Shop Titles")]
        public string level1Title = "Shop I";
        public string level2Title = "Shop II";
        public string level3Title = "Shop III";
        public string level4Title = "Shop IV";
        public string level5Title = "Shop V";

        [Header("Shop Slots")]
        public int level1Slots = 5;
        public int level2Slots = 6;
        public int level3Slots = 7;
        public int level4Slots = 8;
        public int level5Slots = 10;

        [Header("Interest")]
        public float level1Interest = 0.0f;
        public float level2Interest = 2.0f;
        public float level3Interest = 2.5f;
        public float level4Interest = 3.0f;
        public float level5Interest = 3.5f;

        //Sell slot
        private static ShopSellSlot seller;

        //Mono
        private void Awake()
        {
            seller = sellSlot;
        }
        private void OnEnable()
        {
            //Repopulate slots
            selection.PopulateSlots();

            //Set button text
            UpdateUpgradeButton();
        }

        //Access
        public void Reroll()
        {
            //Need at least 1 gold to reroll
            if (state.Gold == 0) return;

            //Pay 1 gold
            state.Gold -= 1;

            //Repopulate slots
            selection.PopulateSlots();
        }
        public static void SetSellState(bool value)
        {
            seller.gameObject.SetActive(value);
        }
        public void UpgradeShop()
        {
            //Get cost
            int cost = ShopUpgradeCost();

            //Need enough gold
            if (state.Gold < cost) return;

            //Pay 1 gold
            state.Gold -= cost;

            //Upgrade shop
            state.ShopLevel++;

            //Increas slot count
            selection.SlotCount = ShopSlotCount();

            //Update button
            UpdateUpgradeButton();
        }

        //Interest
        public void PayInterest()
        {
            state.Gold += GetInterest();
        }
        public int GetInterest()
        {
            float interestRate = ShopInterestRate();
            return Mathf.CeilToInt((state.Gold / 100.0f) * interestRate);
        }

        //Utility
        private void UpdateUpgradeButton()
        {
            int cost = ShopUpgradeCost();
            upgradeButton.Cost = cost;
            upgradeCost.text = cost.ToString();
            title.text = ShopTitle();
        }

        private string ShopTitle()
        {
            switch (state.ShopLevel)
            {
                default:
                case 1: return level1Title;
                case 2: return level2Title;
                case 3: return level3Title;
                case 4: return level4Title;
                case 5: return level5Title;
            }
        }
        private float ShopInterestRate()
        {
            switch (state.ShopLevel)
            {
                default:
                case 1: return level1Interest;
                case 2: return level2Interest;
                case 3: return level3Interest;
                case 4: return level4Interest;
                case 5: return level5Interest;
            }
        }
        private int ShopUpgradeCost()
        {
            switch (state.ShopLevel)
            {
                default:
                case 1: return 5;
                case 2: return 15;
                case 3: return 45;
                case 4: return 150;
                case 5: return 999999999;
            }
        }
        private int ShopSlotCount()
        {
            switch (state.ShopLevel)
            {
                default:
                case 1: return level1Slots;
                case 2: return level2Slots;
                case 3: return level3Slots;
                case 4: return level4Slots;
                case 5: return level5Slots;
            }
        }
    }
}