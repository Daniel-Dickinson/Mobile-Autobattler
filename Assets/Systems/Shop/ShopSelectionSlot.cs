using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class ShopSelectionSlot : ShopUnitSlot
    {
        [Header("State")]
        [SerializeField] private PlayerState state;

        [Header("UI ELements")]
        [SerializeField] private CanvasGroup locked;
        [SerializeField] private CanvasGroup background;

        [Header("Cost")]
        [SerializeField] private TextMeshProUGUI cost;

        //Lock
        public bool Locked
        {
            get { return isLocked; }
        }
        private bool isLocked = false;

        //Mono
        private void Start()
        {
            state.OnGoldChange += QueryState;
        }
        private void OnDestroy()
        {
            state.OnGoldChange -= QueryState;
        }

        //Query State
        private void QueryState()
        {
            bool active = icon != null && state.Gold >= icon.Cost;

            background.alpha = active ? 1.0f : 0.0f;
            hover.gameObject.SetActive(active);
        }

        //Take
        public override bool CanPlaceUnit(ShopUnitIcon icon)
        {
            if (!base.CanPlaceUnit(icon)) return false;

            //Don't accept dragged units
            return (drag == null);
        }
        public override bool CanRemoveUnit()
        {
            if (!base.CanRemoveUnit()) return false;

            //Can only take if cost is met
            return (state.Gold >= icon.Cost);
        }

        //Clear
        public void DirectClear()
        {
            //Icon required to clear it
            if (icon == null) return;

            //Destroy icon
            Destroy(icon.gameObject);

            //Clear icon
            icon = null;

            //No longer locked
            isLocked = false;

            //Update lock
            UpdateLockState();

            //Query state
            QueryState();
        }

        //Place/Remove
        public override bool PlaceUnit(ShopUnitIcon icon)
        {
            //Base
            if (!base.PlaceUnit(icon)) return false;

            //Set cost
            cost.transform.parent.gameObject.SetActive(true);
            cost.text = icon.Cost.ToString();

            //Query state
            QueryState();

            //Success
            return true;
        }
        public override bool RemoveUnit()
        {
            //Icon required
            if (icon == null) return false;

            //Cache icon
            ShopUnitIcon cache = icon;

            //Base
            if (!base.RemoveUnit()) return false;

            //Pay cost
            state.Gold -= cache.Cost;

            //Hide cost
            cost.transform.parent.gameObject.SetActive(false);
            cost.text = "0";

            //No longer locked
            isLocked = false;

            //Update lock
            UpdateLockState();

            //Query state
            QueryState();

            //Success
            return true;
        }

        //Lock
        public void ToggleLock()
        {
            if (!isLocked && icon != null) isLocked = true;
            else isLocked = false;

            //Update
            UpdateLockState();
        }
        private void UpdateLockState()
        {
            locked.alpha = isLocked ? 1.0f : 0.0f;
        }
    }
}