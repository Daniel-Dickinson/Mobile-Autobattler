using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;
using TwoBears.Persistance;
using TwoBears.Unit;
using System;

namespace TwoBears.Loop
{
    public class LoopManager : MonoBehaviour
    {
        [Header("Spawners")]
        public FormationSpawn player;
        public FormationSpawn hostile;

        [Header("Waves")]
        public ProceduralFormation waves;
        
        [Header("UI Elements")]
        public GameObject shop;
        public GameObject complete;
        public GameObject defeat;

        //State Access
        public static LoopStage Stage
        {
            get { return stage; }
        }

        //Event Access
        public static Action OnStageChange;

        //Stage
        private static LoopStage stage;

        //Tracking
        private int playerRemaining;
        private int hostileRemaining;

        //Mono
        private void OnEnable()
        {
            //Start at shop
            ShowShop();
        }
        private void Update()
        {
            //Track remaining units
            TrackUnits();
        }

        //UI Access
        public void ShowShop()
        {
            //Set stage
            stage = LoopStage.Shop;
            OnStageChange?.Invoke();

            //Switch UIs
            shop.SetActive(true);
            complete.SetActive(false);
            defeat.SetActive(false);
        }
        public void RestartShowShop()
        {
            //Clear wave & run
            PersistanceManager.ClearWaves();
            PersistanceManager.ClearRun();

            //Reset waves
            PersistanceManager.State.Wave = 0;

            ShowShop();
        }
        public void StartWave()
        {
            //Close shop
            shop.SetActive(false);

            //Populate formation
            waves.Populate(PersistanceManager.State.Wave);

            //Spawn units
            PersistanceManager.SetupWaves();

            //Track units
            TrackHostileUnits();
            TrackPlayerUnits();

            //Set stage
            stage = LoopStage.Battle;
            OnStageChange?.Invoke();
        }

        //UI Internal
        private void ShowComplete()
        {
            //Show UI
            complete.SetActive(true);

            //Clear wave
            PersistanceManager.ClearWaves();

            //Increment wave
            PersistanceManager.State.Wave++;

            //Set stage
            stage = LoopStage.Complete;
            OnStageChange?.Invoke();
        }
        private void ShowDefeat()
        {
            //Show UI
            defeat.SetActive(true);

            //Set stage
            stage = LoopStage.Defeat;
            OnStageChange?.Invoke();
        }

        //Unit tracking
        private void TrackUnits()
        {
            if (stage != LoopStage.Battle) return;

            //Defeat if 0 player units remaining
            if (playerRemaining == 0)
            {
                ShowDefeat();
                return;
            }

            //Complete if 0 hostile units remaining
            if (hostileRemaining == 0) ShowComplete();
        }

        private void TrackPlayerUnits()
        {
            //Get player units
            BaseUnit[] units = player.GetComponentsInChildren<BaseUnit>();

            //Set player unit count
            playerRemaining = units.Length;

            //Subscribe to killed event
            for (int i = 0; i < units.Length; i++) units[i].OnDeath += PlayerUnitKilled;
        }
        private void TrackHostileUnits()
        {
            //Get player units
            BaseUnit[] units = hostile.GetComponentsInChildren<BaseUnit>();

            //Set player unit count
            hostileRemaining = units.Length;

            //Subscribe to killed event
            for (int i = 0; i < units.Length; i++) units[i].OnDeath += HostileUnitKilled;
        }

        private void PlayerUnitKilled(BaseUnit unit)
        {
            playerRemaining--;
        }
        private void HostileUnitKilled(BaseUnit unit)
        {
            hostileRemaining--;
        }
    }

    public enum LoopStage { Shop, Battle, Complete, Defeat }
}