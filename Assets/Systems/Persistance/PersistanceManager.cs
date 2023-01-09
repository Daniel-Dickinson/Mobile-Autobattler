using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Persistance
{
    public class PersistanceManager : MonoBehaviour
    {
        //Player state
        [SerializeField] private PlayerState data;

        #if UNITY_EDITOR
        [SerializeField] private bool resetOnExit;
        #endif

        //Player data
        public static PlayerState State
        {
            get { return state; }
        }
        private static PlayerState state;

        //Events
        public static Action OnSave;
        public static Action OnLoad;

        public static Action OnClear;
        public static Action OnSetup;

        public static Action OnRunClear;

        //Mono
        private void Awake()
        {
            //Set as static
            state = data;
        }
        private void Start()
        {
            Load();
        }
        private void OnDestroy()
        {
            #if UNITY_EDITOR
            if (resetOnExit) ClearRun();
            #endif
        }

        //Serialization
        public static void Save()
        {
            //Save event
            OnSave?.Invoke();
            state.Save();
        }
        public static void Load()
        {
            OnLoad?.Invoke();
            state.Load();
        }

        //Waves
        public static void ClearWaves()
        {
            OnClear?.Invoke();
        }
        public static void SetupWaves()
        {
            OnSetup?.Invoke();

            //Save on wave start
            Save();
        }

        //Run Progress
        public static void ClearRun()
        {
            //Reset state
            state.Restart();

            //Clear run
            OnRunClear?.Invoke();

            //Save changes
            Save();
        }
    }
}