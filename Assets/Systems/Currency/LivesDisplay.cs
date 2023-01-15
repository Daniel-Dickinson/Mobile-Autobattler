using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Currency
{
    public class LivesDisplay : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerState state;

        [Header("Displays")]
        [SerializeField] private CanvasGroup one;
        [SerializeField] private CanvasGroup two;
        [SerializeField] private CanvasGroup three;

        private void OnEnable()
        {
            state.OnLifeChange += UpdateDisplay;

            UpdateDisplay();
        }
        private void OnDisable()
        {
            state.OnLifeChange -= UpdateDisplay;
        }

        //Core
        private void UpdateDisplay()
        {
            one.alpha = state.Lives >= 1 ? 1.0f : 0.0f;
            two.alpha = state.Lives >= 2 ? 1.0f : 0.0f;
            three.alpha = state.Lives >= 3 ? 1.0f : 0.0f;
        }
    }
}
