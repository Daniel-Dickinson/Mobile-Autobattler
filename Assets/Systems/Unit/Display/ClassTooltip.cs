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
        private static UnitClass selection;
        private static Action onSelectionChange;

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
            //Toggle selection
            if (selection != unitClass) selection = unitClass;
            else selection = UnitClass.None;

            //Selection changed
            onSelectionChange?.Invoke();
        }
        public static void ClearSelection()
        {
            selection = UnitClass.None;
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