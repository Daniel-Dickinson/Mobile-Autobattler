using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class ClassTooltip : MonoBehaviour
    {   
        [Header("Class")]
        [SerializeField] private UnitClass unitClass;

        [Header("Components")]
        [SerializeField] private GameObject tooltip;
        [SerializeField] private CanvasGroup selected;

        //Shared
        public static UnitClass selection;
        public static Action onSelectionChange;

        //Mono
        private void Awake()
        {
            onSelectionChange += UpdateDisplay;
        }
        private void OnDestroy()
        {
            onSelectionChange -= UpdateDisplay;
        }

        //Selection
        public void SetSelected()
        {
            //Set selection
            selection = unitClass;

            //Selection changed
            onSelectionChange?.Invoke();
        }

        //Display
        private void UpdateDisplay()
        {
            tooltip.SetActive(selection == unitClass);
            selected.alpha = (selection == unitClass) ? 1.0f : 0.0f;
        }
    }
}