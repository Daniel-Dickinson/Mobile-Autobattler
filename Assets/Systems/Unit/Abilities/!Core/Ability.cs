using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Waves;
using TwoBears.Pooling;

namespace TwoBears.Unit
{
    public abstract class Ability : Poolable
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

        //Poolable
        public override void PoolableReset()
        {
            base.PoolableReset();

            //Reset buffs
            healIncrease = 0;
            damageIncrease = 0;
            aoeMultiplier = 1.0f;
        }

        //Core
        public abstract bool IsTargetValid(Perceivable self, Perceivable target);
        public abstract void Trigger(Perceivable self, Perceivable target);

        //Summoning
        protected void SummonUnit(GameObject summon, Vector3 position, Vector3 direction, FormationSpawn spawner, Faction faction)
        {
            //Spawn unit
            GameObject minion = Instantiate(summon, position, Quaternion.LookRotation(Vector3.forward, direction), spawner.transform);

            //Set faction
            Perceivable perceivable = minion.GetComponent<Perceivable>();
            perceivable.Faction = faction;

            //Register to formation
            spawner.RegisterSpawn(minion.GetComponent<BaseUnit>());

        }
    }
}