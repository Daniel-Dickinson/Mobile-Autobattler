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

        [Header("Icon")]
        public RectTransform icon;
        public float scale1 = 1.0f;
        public float scale2 = 1.25f;
        public float scale3 = 1.5f;

        //Mono
        private void Awake()
        {
            if (unit != null) unit.OnStack += SetIconScale;
            SetIconScale();
        }

        //Access
        public int Cost
        {
            get { return shopLevel * 3; }
        }
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
            if (icon == null) return;

            //Scale unit icon
            switch (unit.level)
            {
                default:
                case 0:
                    icon.localScale = new Vector3(scale1, scale1, scale1);
                    break;
                case 1:
                    icon.localScale = new Vector3(scale2, scale2, scale2);
                    break;
                case 2:
                    icon.localScale = new Vector3(scale3, scale3, scale3);
                    break;

            }
        }
    }
}