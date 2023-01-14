using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Unit;
using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class ShopTooltip : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;

        [Header("Icon")]
        [SerializeField] private RectTransform iconParent;

        [Header("Info")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private TextMeshProUGUI damage;
        [SerializeField] private TextMeshProUGUI health;

        [Header("Classes")]
        [SerializeField] private RectTransform classParent;
        [SerializeField] private ClassDisplay[] classDisplays;

        [Header("Particles")]
        [SerializeField] private GameObject particles2;
        [SerializeField] private GameObject particles3;
        [SerializeField] private GameObject particles4;
        [SerializeField] private GameObject particles5;

        //Core
        public void SetUnit(ShopUnitIcon icon)
        {
            //Update icon
            SetIcon(icon);

            //Update info
            SetInfo(icon);

            //Update stats
            SetStats(icon);

            //Update class display
            UpdateClassDisplay(icon);

            //Update particles
            UpdateParticles(icon.ShopLevel);
        }

        //Icon
        private void SetIcon(ShopUnitIcon icon)
        {
            //Destroy all existing icons
            if (iconParent.childCount > 0)
            {
                for (int i = iconParent.childCount - 1; i >= 0; i--)
                {
                    Destroy(iconParent.GetChild(i).gameObject);
                }
            }

            //Instantiate copy of icon
            Instantiate(icon.GetIcon(0), iconParent);
        }

        //Info
        private void SetInfo(ShopUnitIcon icon)
        {
            title.text = icon.title;
            description.text = icon.description;
        }

        //Stats
        private void SetStats(ShopUnitIcon icon)
        {
            cost.text = icon.CostPerLevel(0) + " / " + icon.CostPerLevel(1) + " / " + icon.CostPerLevel(2);
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
        private void UpdateClassDisplay(ShopUnitIcon icon)
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