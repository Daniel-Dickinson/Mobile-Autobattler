using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    public abstract class Ability : MonoBehaviour
    {
        public abstract bool IsTargetValid(Perceivable self, Perceivable target);
        public abstract void Trigger(Perceivable target);
    }
}