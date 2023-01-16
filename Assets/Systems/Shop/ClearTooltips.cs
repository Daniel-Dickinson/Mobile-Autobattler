using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;
using TwoBears.Relics;

namespace TwoBears.Shop
{
    public class ClearTooltips : MonoBehaviour
    {
        [Header("Shop")]
        [SerializeField] private ShopSelectionRow selectionRow;

        public void ClearAllTooltips()
        {
            //Clear class tooltip
            ClassTooltip.ClearSelection();

            //Clear unit tooltip
            selectionRow.ClearSelection();

            //Clear relic tooltip
            Relic.CloseAllTooltips();
        }
    }
}