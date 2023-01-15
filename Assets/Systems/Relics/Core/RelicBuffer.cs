using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;
using TwoBears.Persistance;

namespace TwoBears.Relics
{
    public class RelicBuffer : UnitBuffer
    {
        //Apply
        protected override void ApplyBuff(BaseUnit unit)
        {
            foreach (Relic relic in state.GetRelicObjects) relic.ApplyBuff(this, unit);
        }
        protected override void ApplyBuffs(List<BaseUnit> units)
        {
            foreach (Relic relic in state.GetRelicObjects) relic.ApplyBuffs(this, units);
        }
    }
}