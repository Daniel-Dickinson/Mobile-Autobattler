using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Shop
{
    public class ClearTooltips : MonoBehaviour
    {
        [Header("Shop")]
        [SerializeField] private ShopSelectionRow selectionRow;

        public void ClearAllTooltips()
        {
            //Clear class tooltip
            ClassTooltip.selection = UnitClass.None;
            ClassTooltip.onSelectionChange?.Invoke();

            //Clear unit tooltip
            selectionRow.ClearSelection();
        }
    }
}