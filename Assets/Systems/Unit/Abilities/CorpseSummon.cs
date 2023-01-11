using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Waves;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(AudioSource))]
    public class CorpseSummon : Ability
    {
        [Header("Stats")]
        [SerializeField][Range(1, 9)] private int count = 1;
        [SerializeField] private GameObject summon;
        [SerializeField] private float radius = 0.5f;

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
            //Grab unit
            BaseUnit unit = target.GetComponent<BaseUnit>();

            //Check if unit is dead
            if (unit.enabled) return false;

            //Target is valid
            return true;
        }
        public override void Trigger(Perceivable self, Perceivable target)
        {
            //Grab formation spawner
            FormationSpawn spawn = self.GetComponentInParent<FormationSpawn>();

            //Insantiate units
            if (count == 1) SummonUnit(summon, target.transform.position, target.transform.up, spawn, self.Faction);
            else
            {
                float summonSegment = 360.0f / count;
                for (int i = 0; i < count; i++)
                {
                    float angle = summonSegment * i;
                    Vector3 positionOffset = Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
                    SummonUnit(summon, target.transform.position + positionOffset, positionOffset.normalized, spawn, self.Faction);
                }
            }

            //Destroy corpse
            Destroy(target.gameObject);

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