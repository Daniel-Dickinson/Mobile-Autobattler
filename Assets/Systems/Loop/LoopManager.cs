using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Waves;
using TwoBears.Perception;
using TwoBears.Persistance;
using TwoBears.Unit;
using System;

namespace TwoBears.Loop
{
    public class LoopManager : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;

        [Header("Spawners")]
        public FormationSpawn player;
        public FormationSpawn hostile;

        [Header("Waves")]
        public ProceduralFormation waves;
        
        [Header("UI Elements")]
        public GameObject shop;
        public CompleteUI complete;
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
        private void Awake()
        {
            //Attach to summon events
            player.OnSummon += TrackPlayerSummon;
            hostile.OnSummon += TrackHostileSummon;
        }
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
        private void OnDestroy()
        {
            //Detach to summon events
            player.OnSummon -= TrackPlayerSummon;
            hostile.OnSummon -= TrackHostileSummon;
        }

        //UI Access
        public void ShowShop()
        {
            //Set stage
            stage = LoopStage.Shop;
            OnStageChange?.Invoke();

            //Switch UIs
            shop.SetActive(true);
            complete.gameObject.SetActive(false);
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
        private IEnumerator DelayedComplete(float delay)
        {
            //Set stage
            stage = LoopStage.Complete;

            //Wait for delay
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            //Show complete
            ShowComplete();
        }
        private IEnumerator DelayedDefeat(float delay)
        {
            //Set stage
            stage = LoopStage.Complete;

            //Wait for delay
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            //Show defeat
            ShowDefeat();
        }
        private void ShowComplete()
        {
            //Show UI
            complete.gameObject.SetActive(true);

            //Initialize UI
            complete.OnComplete();

            //Clear wave
            PersistanceManager.ClearWaves();

            //Increment wave
            PersistanceManager.State.Wave++;
            
            //Stage change
            OnStageChange?.Invoke();
        }
        private void ShowDefeat()
        {
            //Check if we have enough lives to continue
            if (state.Lives > 1)
            {
                //Show UI
                complete.gameObject.SetActive(true);

                //Initialize UI
                complete.OnDefeat();

                //Clear wave & don't increment
                PersistanceManager.ClearWaves();
            }
            else
            {
                //Show UI
                defeat.SetActive(true);
            }

            //Reduce lives
            state.Lives -= 1;

            //State change
            OnStageChange?.Invoke();
        }

        //Unit tracking
        private void TrackUnits()
        {
            if (stage != LoopStage.Battle) return;

            //Defeat if 0 player units remaining
            if (playerRemaining == 0)
            {
                StartCoroutine(DelayedDefeat(0.5f));
                return;
            }

            //Complete if 0 hostile units remaining
            if (hostileRemaining <= 0) StartCoroutine(DelayedComplete(0.5f));
        }

        //Setup
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

        //Summons
        private void TrackPlayerSummon(BaseUnit unit)
        {
            if (unit == null) return;

            playerRemaining++;
            unit.OnDeath += PlayerUnitKilled;
        }
        private void TrackHostileSummon(BaseUnit unit)
        {
            if (unit == null) return;

            hostileRemaining++;
            unit.OnDeath += HostileUnitKilled;
        }

        //Events
        private void PlayerUnitKilled(BaseUnit unit)
        {
            playerRemaining--;
            if (unit != null) unit.OnDeath -= PlayerUnitKilled;
        }
        private void HostileUnitKilled(BaseUnit unit)
        {
            hostileRemaining--;
            if (unit != null) unit.OnDeath -= HostileUnitKilled;
        }
    }

    public enum LoopStage { Shop, Battle, Complete, Defeat }
}