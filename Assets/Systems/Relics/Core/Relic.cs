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

        //Access
        public int ID
        {
            get { return id; }
        }

        //Event Access
        public Action OnPress;

        //Components
        private Button button;

        //Mono
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Press);
        }
        private void OnDestroy()
        {
            button.onClick.RemoveListener(Press);
        }

        //Core
        public abstract void ApplyBuff(RelicBuffer buffer, BaseUnit unit);
        public abstract void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units);

        //UI
        private void Press()
        {
            OnPress?.Invoke();
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