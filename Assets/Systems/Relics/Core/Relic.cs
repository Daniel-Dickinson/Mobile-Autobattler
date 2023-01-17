using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwoBears.Unit;

namespace TwoBears.Relics
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class Relic : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private int id;

        [Header("Info")]
        public string title;
        [TextArea(4, 5)] public string information;

        [Header("Selection")]
        public CanvasGroup darkener;

        [Header("Tooltip")]
        public GameObject tooltip;

        //Access
        public int ID
        {
            get { return id; }
        }

        //Event Access
        public Action OnPress;

        //Components
        private Button button;

        //Tooltips
        private bool tooltipEnabled = false;

        private static int selectedRelic = -1;
        private static Action OnRelicSelected;

        //Mono
        private void Awake()
        {
            //Grab button & attach
            button = GetComponent<Button>();
            button.onClick.AddListener(Press);

            //Attach to relic selected event
            OnRelicSelected += UpdateTooltip;

            //Disable tooltip by default
            tooltip.SetActive(false);
        }
        private void OnDestroy()
        {
            //Detach to relic selected event
            OnRelicSelected += UpdateTooltip;

            //Detach from button event
            button.onClick.RemoveListener(Press);
        }

        //Core
        public abstract void ApplyBuff(RelicBuffer buffer, BaseUnit unit);
        public abstract void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units);

        //UI
        private void Press()
        {
            OnPress?.Invoke();

            //Toggle tooltip
            ToggleTooltip();
        }
        public void EnableTooltip()
        {
            tooltipEnabled = true;

            //Darken tooltip
            darkener.alpha = 1.0f;
        }

        private void ToggleTooltip()
        {
            if (tooltip == null) return;
            if (!tooltipEnabled) return;

            if (selectedRelic == id)
            {
                selectedRelic = -1;
                OnRelicSelected?.Invoke();
            }
            else
            {
                selectedRelic = id;
                OnRelicSelected?.Invoke();
            }
        }
        private void UpdateTooltip()
        {
            if (tooltip == null) return;

            //Update display
            if (tooltipEnabled)
            {
                tooltip.SetActive(selectedRelic == id);
                darkener.alpha = (selectedRelic == id)? 0.0f: 1.0f;
            }
        }

        public static void CloseAllTooltips()
        {
            selectedRelic = -1;
            OnRelicSelected?.Invoke();
        }
        
        //Utility
        protected bool IsClass(BaseUnit unit, UnitClass unitClass)
        {
            if (unit.primary == unitClass) return true;
            if (unit.secondary == unitClass) return true;
            if (unit.tertiary == unitClass) return true;

            return false;
        }
    }
}