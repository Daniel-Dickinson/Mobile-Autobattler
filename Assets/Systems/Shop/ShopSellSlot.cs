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
            int refund = icon.Cost;
            if (icon.Unit.level > 0) refund *= 3;
            if (icon.Unit.level > 1) refund *= 3;

            PersistanceManager.State.Gold += refund;

            //Destroy icon
            Destroy(icon.gameObject);

            //Clear icon
            icon = null;

            //Success
            return true;
        }
    }
}