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
        public Color standard;
        public Color stunned;

        [Header("Enemy")]
        public Color enemy;
        public Color killed;

        private SpriteRenderer rend;
        private Perceiver perceiver;
        private BaseUnit unit;

        private void OnEnable()
        {
            perceiver = GetComponent<Perceiver>();
            rend = GetComponent<SpriteRenderer>();
            unit = GetComponent<BaseUnit>();

            unit.OnDamaged += SetColor;
            unit.OnHealed += SetColor;
        }
        private void OnDisable()
        {
            if (unit == null) return;

            unit.OnDamaged -= SetColor;
            unit.OnHealed -= SetColor;
        }
        private void Start()
        {
            SetColor(unit);
        }

        private void SetColor(BaseUnit unit)
        {
            //Calculate lerp
            float l = Mathf.InverseLerp(0, unit.MaxHealth, unit.Health);

            //Set color
            switch (perceiver.Faction)
            {
                case Faction.Player:
                    rend.color = Color.Lerp(stunned, standard, l);
                    break;
                case Faction.Hostile:
                    rend.color = Color.Lerp(killed, enemy, l);
                    break;
            }

            //Set to back if killed
            if (l == 0) rend.sortingOrder = -50;
        }
    }
}