using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class ShopSellSlot : ShopUnitSlot
    {
        public override bool PlaceUnit(ShopUnitIcon icon)
        {
            //Base
            if (!base.PlaceUnit(icon)) return false;

            //Refund cost
            PersistanceManager.State.Gold += icon.Cost;

            //Destroy icon
            Destroy(icon.gameObject);

            //Clear icon
            icon = null;

            //Success
            return true;
        }
    }
}