using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;
using TwoBears.Perception;
using UnityEngine.Events;

namespace TwoBears.Waves
{
    public class FormationSpawn : MonoBehaviour
    {
        [Header("Formation")]
        public Formation formation; 
        
        [Header("Selection")]
        public UnitSelection selection;

        [Header("Faction")]
        public Faction faction;
        public float xSeperation = 1.0f;
        public float ySeperation = 0.75f;

        //Events
        public Action OnSpawn;
        public UnitEvent OnSummon;

        //Spawns
        public List<BaseUnit> Spawns
        {
            get { return spawns; }
        }
        private List<BaseUnit> spawns;

        //Mono
        private void OnEnable()
        {
            Register();
        }
        private void OnDisable()
        {
            Deregister();
        }

        //Registration
        private void Register()
        {
            SpawnManager.RegisterFormation(this);
        }
        private void Deregister()
        {
            SpawnManager.DeregisterFormation(this);
        }

        //Core
        public void Clear()
        {
            //Spawns required
            if (spawns == null) return;

            //Destroy all children
            for (int i = transform.childCount - 1; i >= 0; i--) Destroy(transform.GetChild(i).gameObject);

            //Clear
            spawns.Clear();
        }
        public void Spawn()
        {
            //Front
            for (int x = 0; x < formation.front.Length; x++)
            {
                //Unit must be valid
                if (formation.front[x] == null) continue;

                //Calculate local position
                float localX = (x - (formation.front.Length / 2.0f) + 0.5f) * xSeperation;
                float localY = 0;

                //Spawn unit
                SpawnUnit(formation.front[x], new Vector3(localX, localY, 0));
            }

            //Middle
            for (int x = 0; x < formation.middle.Length; x++)
            {
                //Unit must be valid
                if (formation.middle[x] == null) continue;

                //Calculate local position
                float localX = (x - (formation.middle.Length / 2.0f) + 0.5f) * xSeperation;
                float localY = -ySeperation;

                //Spawn unit
                SpawnUnit(formation.middle[x], new Vector3(localX, localY, 0));
            }

            //Back
            for (int x = 0; x < formation.back.Length; x++)
            {
                //Unit must be valid
                if (formation.back[x] == null) continue;

                //Calculate local position
                float localX = (x - (formation.back.Length / 2.0f) + 0.5f) * xSeperation;
                float localY = -ySeperation * 2.0f;

                //Spawn unit
                SpawnUnit(formation.back[x], new Vector3(localX, localY, 0));
            }

            //Spawn event
            OnSpawn?.Invoke();
        }
        public void SpawnUnit(FormationUnit unit, Vector3 localPosition)
        {
            //Initialize spawns
            if (spawns == null) spawns = new List<BaseUnit>();

            //Determine source
            GameObject source = selection.GetUnit(unit.id, unit.level);
            if (source == null) return;

            //Spawn unit
            GameObject spawn = Instantiate(source, transform.position + (transform.rotation * localPosition), transform.rotation, transform);

            //Grab Perceiver & set faction
            Perceiver perceiver = spawn.GetComponent<Perceiver>();
            if (perceiver != null) perceiver.Faction = faction;
            else
            {
                //Grab all child units if swarm
                Perceiver[] perceivers = spawn.GetComponentsInChildren<Perceiver>();
                for (int i = 0; i < perceivers.Length; i++) perceivers[i].Faction = faction;
            }

            //Rename
            spawn.name = source.name;

            //Track unit
            BaseUnit spawnedUnit = spawn.GetComponent<BaseUnit>();
            if (spawnedUnit != null) spawns.Add(spawnedUnit);
            else
            {
                //Track swarm units
                BaseUnit[] spawnedUnits = spawn.GetComponentsInChildren<BaseUnit>();
                for (int i = 0; i < spawnedUnits.Length; i++) spawns.Add(spawnedUnits[i]);
            }
        }

        //Summons
        public void RegisterSpawn(BaseUnit unit)
        {
            //Valid unit required
            if (unit == null) return;

            //Add unit -- can now be cleared at end of round
            spawns.Add(unit);

            //On Summon
            OnSummon?.Invoke(unit);
        }

        //Debug
        public void KillAll()
        {
            //Spawns required
            if (spawns == null) return;

            //Destroy spawn
            for (int i = 0; i < spawns.Count; i++)
            {
                BaseUnit unit = spawns[i].GetComponent<BaseUnit>();
                if (unit != null) unit.RemoveHealth(99);
            }
        }
    }
}