using System.Collections;
using System.Collections.Generic;
using TwoBears.Waves;
using UnityEngine;

namespace TwoBears.Shop
{
    [CreateAssetMenu(menuName = "TwoBears/ShopUnits")]
    public class ShopUnitPool : ScriptableObject
    {
        public ShopUnitIcon[] units;

        public ShopUnitIcon GetUnit(FormationUnit unit)
        {
            if (unit == null || unit.id < 0) return null;

            //Search for unit with matching ID
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].Unit.id == unit.id) return units[i];
            }

            //Return default unit
            return units[0];
        }
        public ShopUnitIcon GetRandomUnit(int shopLevel)
        {
            //Grab random unit
            int index = Random.Range(0, units.Length);
            ShopUnitIcon unit = units[index];

            //If unit is above current shop level grab another
            while (unit.ShopLevel > shopLevel)
            {
                index = Random.Range(0, units.Length);
                unit = units[index];
            }

            //Return unit
            return units[index];
        }
    }
}