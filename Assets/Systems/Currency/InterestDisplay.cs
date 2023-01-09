using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;

namespace TwoBears.Currency
{
    public class InterestDisplay : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerState state;
        [SerializeField] private Shop.Shop shop;

        [Header("Components")]
        [SerializeField] private GameObject body;
        [SerializeField] private TextMeshProUGUI display;

        private void OnEnable()
        {
            state.OnGoldChange += UpdateDisplay;
            state.OnShopLevelChange += UpdateDisplay;

            UpdateDisplay();
        }
        private void OnDisable()
        {
            state.OnGoldChange -= UpdateDisplay;
            state.OnShopLevelChange -= UpdateDisplay;
        }

        //Core
        private void UpdateDisplay()
        {
            int interest = shop.GetInterest();

            display.text = interest.ToString();
            body.SetActive(interest != 0);
        }
    }
}