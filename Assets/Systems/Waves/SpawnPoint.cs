using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Waves
{
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Selection")]
        public GameObject unit;

        [Header("Setup")]
        public Faction faction;
        public int x, y;

        //Tracked unit
        private GameObject spawn;

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
            SpawnManager.RegisterSpawn(this);
        }
        private void Deregister()
        {
            SpawnManager.DeregisterSpawn(this);
        }

        //Core
        public void Clear()
        {
            if (spawn == null) return;

            //Destroy spawn
            Destroy(spawn);
        }
        public void Spawn()
        {
            //Spawn unit
            spawn = Instantiate(unit, transform.position, transform.rotation, transform);

            //Grab Perceiver & set faction
            Perceiver perceiver = spawn.GetComponent<Perceiver>();
            perceiver.Faction = faction;

            //Rename
            spawn.name = unit.name;
        }
    }
}