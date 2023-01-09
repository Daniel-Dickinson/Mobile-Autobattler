using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;
using TwoBears.Waves;

namespace TwoBears.Shop
{
    public class ShopFormationSlot : ShopUnitSlot
    {
        [Header("Stacks")]
        public CanvasGroup One;
        public CanvasGroup Two;

        private PlayerState state;
        private FormationRow row;
        private int position;

        //Initialization
        public void Initialize(FormationRow row, int position, PlayerState state, ShopUnitPool pool)
        {
            this.row = row;
            this.state = state;
            this.position = position;

            //Get unit
            FormationUnit unit = state.GetFormationUnit(row, position);
            if (unit == null)
            {
                UpdateStacks();
                return;
            }

            //Source unit icon
            ShopUnitIcon source = pool.GetUnit(unit);
            if (source == null)
            {
                UpdateStacks();
                return;
            }

            //Instantiate copy
            ShopUnitIcon newIcon = Instantiate(source);

            //Attach unit to icon
            newIcon.Unit = unit;

            //Place in slot
            base.PlaceUnit(newIcon);

            //Update stacks
            UpdateStacks();
        }

        //Place/Remove
        public override bool PlaceUnit(ShopUnitIcon icon)
        {
            //Base
            if (!base.PlaceUnit(icon)) return false;

            //Place unit in formation
            state.SetFormationUnit(row, position, this.icon.Unit);

            //Update stacks
            UpdateStacks();

            //Success
            return true;
        }
        public override bool RemoveUnit()
        {
            //Base
            if (!base.RemoveUnit()) return false;

            //Clear unit from formation
            state.SetFormationUnit(row, position, null);

            //Update stacks
            UpdateStacks();

            //Success
            return true;
        }

        //Stacks
        public void UpdateStacks()
        {
            if (icon == null)
            {
                One.alpha = 0.0f;
                Two.alpha = 0.0f;

                return;
            }

            One.alpha = (icon.Unit.sublevel > 0) ? 1.0f : 0.3f;
            Two.alpha = (icon.Unit.sublevel > 1) ? 1.0f : 0.3f;
        }
    }
}