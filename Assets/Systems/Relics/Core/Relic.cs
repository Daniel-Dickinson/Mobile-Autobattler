using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Relics
{
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

        //Core
        public abstract void ApplyBuff(RelicBuffer buffer, BaseUnit unit);
        public abstract void ApplyBuffs(RelicBuffer buffer, List<BaseUnit> units);

        //UI
        public void Press()
        {
            OnPress?.Invoke();
        }
    }
}