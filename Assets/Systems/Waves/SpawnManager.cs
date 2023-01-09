using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Waves
{
    [RequireComponent(typeof(PersistanceManager))]
    public class SpawnManager : MonoBehaviour
    {
        //Points
        private static List<SpawnPoint> spawns;
        public static void RegisterSpawn(SpawnPoint spawn)
        {
            if (spawns == null) spawns = new List<SpawnPoint>();
            spawns.Add(spawn);
        }
        public static void DeregisterSpawn(SpawnPoint spawn)
        {
            if (spawns == null) return;
            spawns.Remove(spawn);
        }

        //Formations
        private static List<FormationSpawn> formations;
        public static void RegisterFormation(FormationSpawn formation)
        {
            if (formations == null) formations = new List<FormationSpawn>();
            formations.Add(formation);
        }
        public static void DeregisterFormation(FormationSpawn formation)
        {
            if (formations == null) return;
            formations.Remove(formation);
        }

        //Mono
        private void OnEnable()
        {
            PersistanceManager.OnClear += ClearAll;
            PersistanceManager.OnSetup += SpawnAll;
        }
        private void OnDisable()
        {
            PersistanceManager.OnClear -= ClearAll;
            PersistanceManager.OnSetup -= SpawnAll;
        }

        //Core
        public static void ClearAll()
        {

            if (spawns != null) for (int i = 0; i < spawns.Count; i++) spawns[i].Clear();
            if (formations != null) for (int i = 0; i < formations.Count; i++) formations[i].Clear();
        }
        public static void SpawnAll()
        {
            if (spawns != null) for (int i = 0; i < spawns.Count; i++) spawns[i].Spawn();
            if (formations != null) for (int i = 0; i < formations.Count; i++) formations[i].Spawn();
        }
    }
}