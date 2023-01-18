using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public abstract class PassiveAbility : MonoBehaviour
    {
        //Buff Access
        public int HealIncrease
        {
            get { return healIncrease; }
            set { healIncrease = value; }
        }
        public int DamageIncrease
        {
            get { return damageIncrease; }
            set { damageIncrease = value; }
        }
        public float AOEMultiplier
        {
            get { return aoeMultiplier; }
            set { aoeMultiplier = value; }
        }

        //Buffing
        protected int healIncrease = 0;
        protected int damageIncrease = 0;
        protected float aoeMultiplier = 1.0f;
    }
}