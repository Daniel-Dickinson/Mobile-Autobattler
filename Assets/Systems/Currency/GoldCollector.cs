using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;
using TwoBears.Persistance;
using TwoBears.Waves;

namespace TwoBears.Currency
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(FormationSpawn))]
    public class GoldCollector : MonoBehaviour
    {
        public PlayerState state;

        private AudioSource aud;
        private FormationSpawn spawn;

        //Mono
        private void OnEnable()
        {
            aud = GetComponent<AudioSource>();
            spawn = GetComponent<FormationSpawn>();

            spawn.OnSpawn += AttachToSpawnedUnits;
        }
        private void OnDisable()
        {
            spawn.OnSpawn -= AttachToSpawnedUnits;
        }

        //Attach on spawn
        private void AttachToSpawnedUnits()
        {
            //Get player units
            BaseUnit[] units = spawn.GetComponentsInChildren<BaseUnit>();

            //Subscribe to killed event
            for (int i = 0; i < units.Length; i++) units[i].OnDeath += OnUnitDeath;
        }

        //On Death
        private void OnUnitDeath(BaseUnit unit)
        {
            //Add gold
            state.Gold += 1;

            //Play collect sound
            aud.Play();
        }
    }
}