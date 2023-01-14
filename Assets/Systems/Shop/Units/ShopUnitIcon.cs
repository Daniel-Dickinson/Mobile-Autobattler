using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;

namespace TwoBears.Shop
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ShopUnitIcon : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private FormationUnit unit;
        [Range(1, 5)][SerializeField] private int shopLevel = 1;

        [Header("Info")]
        public string title;
        [TextArea(5, 8)] public string description;

        [Header("Icons")]
        [SerializeField] private RectTransform icon1;
        [SerializeField] private RectTransform icon2;
        [SerializeField] private RectTransform icon3;

        [Header("Scale")]
        [SerializeField] private float scale1 = 1.0f;
        [SerializeField] private float scale2 = 1.05f;
        [SerializeField] private float scale3 = 1.1f;

        //Mono
        private void Awake()
        {
            if (unit != null) unit.OnStack += SetIconScale;
            SetIconScale();
        }

        //Icons
        public GameObject GetIcon(int level)
        {
            switch (level)
            {
                default:
                case 0:
                    return icon1.gameObject;
                case 1:
                    return icon2.gameObject;
                case 2:
                    return icon3.gameObject;
            }
        }

        //Stats
        public int Cost
        {
            get { return shopLevel * 3; }
        }

        //Access
        public FormationUnit Unit
        {
            get { return unit; }
            set
            {
                //Deregister from old unit
                if (unit != null) unit.OnStack -= SetIconScale;

                //Change unit
                unit = value;

                //Set new scale
                SetIconScale();

                //Register to new unit
                if (unit != null) unit.OnStack += SetIconScale;
            }
        }
        public int ShopLevel
        {
            get { return shopLevel; }
        }

        //Visuals
        private void SetIconScale()
        {
            //Valid unit required
            if (unit == null) return;

            //Disable icons
            if (icon1 != null) icon1.gameObject.SetActive(false);
            if (icon2 != null) icon2.gameObject.SetActive(false);
            if (icon3 != null) icon3.gameObject.SetActive(false);

            //Scale unit icon
            switch (unit.level)
            {
                default:
                case 0:
                    if (icon1 == null) return;
                    icon1.gameObject.SetActive(true);
                    icon1.localScale = new Vector3(scale1, scale1, scale1);
                    break;
                case 1:
                    if (icon2 == null) return;
                    icon2.gameObject.SetActive(true);
                    icon2.localScale = new Vector3(scale2, scale2, scale2);
                    break;
                case 2:
                    if (icon3 == null) return;
                    icon3.gameObject.SetActive(true);
                    icon3.localScale = new Vector3(scale3, scale3, scale3);
                    break;

            }
        }
    }
}