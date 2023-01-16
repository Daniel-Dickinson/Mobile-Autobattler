using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwoBears.Waves;
using TwoBears.Unit;

namespace TwoBears.Loop
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BattleBar : MonoBehaviour
    {
        [Header("Formations")]
        [SerializeField] private FormationSpawn player;
        [SerializeField] private FormationSpawn hostile;

        [Header("Bar Metrics")]
        [SerializeField] private float speed = 2.0f;
        [SerializeField] private float smooth = 0.02f;

        [Header("Components")]
        [SerializeField] private Image bar;

        //Components
        private CanvasGroup group;

        //Stats
        private int totalPlayerHealthPool;
        private int totalHostileHealthPool;

        private float velocity = 0.0f;
        private float goal = 0.5f;

        //Mono
        private void Awake()
        {
            //Hide
            group = GetComponent<CanvasGroup>();
            group.alpha = 0.0f;

            //Attach to spawners
            player.TrackSpawn += OnPlayerUnitsSpawned;
            player.TrackSummon += OnPlayerUnitSummoned;

            hostile.TrackSpawn += OnHostileUnitsSpawned;
            hostile.TrackSummon += OnHostileUnitSummoned;
        }
        private void OnDestroy()
        {
            //Detach from spawners
            player.TrackSpawn -= OnPlayerUnitsSpawned;
            player.TrackSummon -= OnPlayerUnitSummoned;

            hostile.TrackSpawn -= OnHostileUnitsSpawned;
            hostile.TrackSummon -= OnHostileUnitSummoned;
        }
        private void Update()
        {
            //Update bar
            UpdateBar(Time.deltaTime);
        }

        public void Show()
        {
            //Show
            group.alpha = 1.0f;
        }
        public void Hide()
        {
            //Hide
            group.alpha = 0.0f;

            //Empty pools
            totalPlayerHealthPool = 0;
            totalHostileHealthPool = 0;
        }

        //Track Callbacks
        private void OnPlayerUnitsSpawned()
        {
            for (int i = 0; i < player.Spawns.Count; i++) OnPlayerUnitSummoned(player.Spawns[i]);
        }
        private void OnHostileUnitsSpawned()
        {
            for (int i = 0; i < hostile.Spawns.Count; i++) OnHostileUnitSummoned(hostile.Spawns[i]);
        }

        //Summon Callbacks
        private void OnPlayerUnitSummoned(BaseUnit unit)
        {
            totalPlayerHealthPool += unit.MaxHealth;

            unit.OnDamaged += OnPlayerHealthLost;
            unit.OnHealed += OnPlayerHealthGained;

            UpdateDisplay();
        }
        private void OnHostileUnitSummoned(BaseUnit unit)
        {
            totalHostileHealthPool += unit.MaxHealth;

            unit.OnDamaged += OnHostileHealthLost;
            unit.OnHealed += OnHostileHealthGained;

            UpdateDisplay();
        }

        //Damage Callbacks
        private void OnPlayerHealthLost(int amount)
        {
            totalPlayerHealthPool -= amount;
            UpdateDisplay();
        }
        private void OnHostileHealthLost(int amount)
        {

            totalHostileHealthPool -= amount;
            UpdateDisplay();
        }

        //Health Callbacks
        private void OnPlayerHealthGained(int amount)
        {
            totalPlayerHealthPool += amount;
            UpdateDisplay();
        }
        private void OnHostileHealthGained(int amount)
        {
            totalHostileHealthPool += amount;
            UpdateDisplay();
        }

        //Display
        private void UpdateDisplay()
        {
            float total = totalHostileHealthPool + totalPlayerHealthPool;
            float lerp = totalPlayerHealthPool / total;

            goal = Mathf.Clamp01(lerp);
        }
        private void UpdateBar(float deltaTime)
        {
            if (group.alpha > 0) bar.fillAmount = Mathf.SmoothDamp(bar.fillAmount, goal, ref velocity, smooth, speed, deltaTime);
        }
    }
}