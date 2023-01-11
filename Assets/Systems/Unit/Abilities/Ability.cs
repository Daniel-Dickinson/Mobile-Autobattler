using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Waves;

namespace TwoBears.Unit
{
    public abstract class Ability : MonoBehaviour
    {
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