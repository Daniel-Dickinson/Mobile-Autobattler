using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;

namespace TwoBears.Currency
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ShardDisplay : MonoBehaviour
    {
        public PlayerState state;
        private TextMeshProUGUI display;

        private void OnEnable()
        {
            display = GetComponent<TextMeshProUGUI>();

            state.OnShardChange += UpdateDisplay;

            UpdateDisplay();
        }
        private void OnDisable()
        {
            state.OnShardChange -= UpdateDisplay;
        }

        //Core
        private void UpdateDisplay()
        {
            display.text = state.Shards.ToString();
        }
    }
}