using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TwoBears.Persistance;

namespace TwoBears.Loop
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class WaveDisplay : MonoBehaviour
    {
        public PlayerState state;

        private TextMeshProUGUI display;

        //Mono
        private void OnEnable()
        {
            display = GetComponent<TextMeshProUGUI>();

            state.OnWaveChange += UpdateDisplay;
        }
        private void OnDisable()
        {
            state.OnWaveChange -= UpdateDisplay;
        }

        //Core
        private void UpdateDisplay()
        {
            display.text = "Wave " + Mathf.Max(1, state.Wave + 1);
        }
    }
}