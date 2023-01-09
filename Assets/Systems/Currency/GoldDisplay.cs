using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;

namespace TwoBears.Currency
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class GoldDisplay : MonoBehaviour
    {
        public PlayerState state;
        private TextMeshProUGUI display;

        private void OnEnable()
        {
            display = GetComponent<TextMeshProUGUI>();

            state.OnGoldChange += UpdateDisplay;

            UpdateDisplay();
        }
        private void OnDisable()
        {
            state.OnGoldChange -= UpdateDisplay;
        }

        //Core
        private void UpdateDisplay()
        {
            display.text = state.Gold.ToString();
        }
    }
}