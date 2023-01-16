using System.Collections;
using System.Collections.Generic;
using TwoBears.Perception;
using UnityEngine;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(BaseUnit))]
    [RequireComponent(typeof(Perceiver))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class HealthIndicator : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Color standard;
        [SerializeField] private Color stunned;

        [Header("Enemy")]
        [SerializeField] private Color enemy;
        [SerializeField] private Color killed;

        public const int frameCount = 6;

        private SpriteRenderer rend;
        private Perceiver perceiver;
        private BaseUnit unit;

        private void Awake()
        {
            perceiver = GetComponent<Perceiver>();
            rend = GetComponent<SpriteRenderer>();
            unit = GetComponent<BaseUnit>();
        }
        private void OnEnable()
        {
            unit.OnDamaged += OnDamage;
            unit.OnHealed += OnHeal;
        }
        private void OnDisable()
        {
            if (unit == null) return;

            unit.OnDamaged -= OnDamage;
            unit.OnHealed -= OnHeal;
        }
        private void Start()
        {
            SetColor(unit);
        }

        private void OnDamage(int delta)
        {
            StopAllCoroutines();
            StartCoroutine(FrameFlash(Color.white));
        }
        private void OnHeal(int delta)
        {
            StopAllCoroutines();

            switch (perceiver.Faction)
            {
                case Faction.Player:
                    StartCoroutine(FrameFlash(standard));
                    break;

                case Faction.Hostile:
                    StartCoroutine(FrameFlash(enemy));
                    break;
            }
        }

        private IEnumerator FrameFlash(Color color)
        {
            //Set custom color
            SetCustomColor(color);

            int frame = frameCount;
            while (frame >= 0)
            {
                frame--;
                yield return new WaitForFixedUpdate();
            }

            //Set back to standard color
            SetColor(unit);
        }

        //Utility
        private void SetColor(BaseUnit unit)
        {
            //Calculate lerp
            float l = Mathf.InverseLerp(0, unit.MaxHealth, unit.Health);

            //Set color
            SetColor(l);
        }
        private void SetColor(float lerp)
        {
            //Set color
            switch (perceiver.Faction)
            {
                case Faction.Player:
                    rend.color = Color.Lerp(stunned, standard, lerp);
                    break;
                case Faction.Hostile:
                    rend.color = Color.Lerp(killed, enemy, lerp);
                    break;
            }

            //Set to back if killed
            if (lerp == 0) rend.sortingOrder = -50;
        }
        private void SetCustomColor(Color color)
        {
            rend.color = color;
        }
    }
}