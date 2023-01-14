using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;
using TwoBears.Unit;

namespace TwoBears.Shop
{
    public class ShopSelectionSlot : ShopUnitSlot
    {
        [Header("State")]
        [SerializeField] private PlayerState state;

        [Header("UI Elements")]
        [SerializeField] private CanvasGroup locked;
        [SerializeField] private CanvasGroup background;

        [Header("Cost")]
        [SerializeField] private TextMeshProUGUI cost;

        [Header("Classes")]
        [SerializeField] private RectTransform classParent;
        [SerializeField] private ClassDisplay[] classDisplays;

        [Header("Particles")]
        [SerializeField] private GameObject particles2;
        [SerializeField] private GameObject particles3;
        [SerializeField] private GameObject particles4;
        [SerializeField] private GameObject particles5;

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

            //Clear class display
            ClearClassDisplay();

            //Updated particles
            UpdateParticles(0);

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

            //Update class display
            UpdateClassDisplay();

            //Updated particles
            UpdateParticles(icon.ShopLevel);

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

            //Clear class displays
            ClearClassDisplay();

            //Updated particles
            UpdateParticles(0);

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

        //Class Display
        private void ClearClassDisplay()
        {
            //Destroy all children
            for (int i = classParent.childCount - 1; i >= 0; i--) Destroy(classParent.GetChild(i).gameObject);
        }
        private void AddClassDisplay(UnitClass unitClass, int upgradeID)
        {
            if (unitClass == UnitClass.None) return;
            for (int i = 0; i < classDisplays.Length; i++)
            {
                if (classDisplays[i].unitClass == unitClass)
                {
                    //Instantiate class display under parent
                    ClassDisplay display = Instantiate(classDisplays[i], classParent);

                    //Set upgrade status
                    display.UnitID = upgradeID;
                    return; 
                }
            }
        }
        private void UpdateClassDisplay()
        {
            //Clear existing
            ClearClassDisplay();

            //Setup new class displays
            if (icon != null)
            {
                int unitId = icon.Unit.id;

                //Get classes
                state.GetClasses(unitId, out UnitClass primary, out UnitClass secondary, out UnitClass tertiary);

                //Add our displays
                AddClassDisplay(primary, unitId);
                AddClassDisplay(secondary, unitId);
                AddClassDisplay(tertiary, unitId);
            }
        }

        //Particles
        private void UpdateParticles(int unitTier)
        {
            switch (unitTier)
            {
                default:
                case 0:
                case 1:
                    particles2.SetActive(false);
                    particles3.SetActive(false);
                    particles4.SetActive(false);
                    particles5.SetActive(false);
                    break;
                case 2:
                    particles2.SetActive(true);
                    particles3.SetActive(false);
                    particles4.SetActive(false);
                    particles5.SetActive(false);
                    break;
                case 3:
                    particles2.SetActive(false);
                    particles3.SetActive(true);
                    particles4.SetActive(false);
                    particles5.SetActive(false);
                    break;
                case 4:
                    particles2.SetActive(false);
                    particles3.SetActive(false);
                    particles4.SetActive(true);
                    particles5.SetActive(false);
                    break;
                case 5:
                    particles2.SetActive(false);
                    particles3.SetActive(false);
                    particles4.SetActive(false);
                    particles5.SetActive(true);
                    break;
            }
        }
    }
}