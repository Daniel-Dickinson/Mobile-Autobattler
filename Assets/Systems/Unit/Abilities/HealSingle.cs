using System.Collections;
using System.Collections.Generic;
using TwoBears.Perception;
using UnityEngine;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(AudioSource))]
    public class HealSingle : Ability
    {
        [Header("Stats")]
        [SerializeField] private int heal;

        [Header("Indicator")]
        [SerializeField] private float minSize = 0.2f;
        [SerializeField] private float maxSize = 1.0f;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float duration = 0.4f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        //Mono
        private void OnEnable()
        {
            //Scale up over time
            StartCoroutine(ScaleUpOverTime(duration));
        }

        //Functionality
        public override bool IsTargetValid(Perceivable self, Perceivable target)
        {
            //Target must be of same faction
            if (target.Faction != self.Faction) return false;

            //Grab unit
            BaseUnit unit = target.GetComponent<BaseUnit>();

            //Target must require healing
            if (unit == null || unit.Health == unit.MaxHealth) return false;

            //Target is valid
            return true;
        }
        public override void Trigger(Perceivable target)
        {
            //Grab unit
            BaseUnit unit = target.GetComponent<BaseUnit>();

            //Heal unit
            unit.RestoreHealth(heal);

            //Play hit
            if (audioSource != null) audioSource.Play();
        }

        //Indicator
        private IEnumerator ScaleUpOverTime(float time)
        {
            float duration = time;

            while (time > 0)
            {
                float lerp = curve.Evaluate(1 - Mathf.Clamp01(time / duration));
                float scale = Mathf.Lerp(minSize, maxSize, lerp);

                //Set scale
                transform.localScale = new Vector3(scale, scale, scale);

                //Reduce time
                time -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            //Destoy
            Destroy(gameObject);

        }
    }
}