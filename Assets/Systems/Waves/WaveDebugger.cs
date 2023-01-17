using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Waves
{
    [RequireComponent(typeof(FormationSpawn))]
    public class WaveDebugger : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private KeyCode clearWave;

        //Components
        private FormationSpawn spawner;

        //Mono
        private void Awake()
        {
            //Grab Spawner
            spawner = GetComponent<FormationSpawn>();
        }
        private void Update()
        {
            DebugInput();
        }

        //Core
        private void DebugInput()
        {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(clearWave)) spawner.KillAll();
        #endif
        }
    }
}