using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Waves;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Explosion))]
    [RequireComponent(typeof(AudioSource))]
    public class CorpseSummon : ChannelAbility
    {
        [Header("Stats")]
        [SerializeField][Range(1, 9)] private int count = 1;
        [SerializeField] private GameObject summon;
        [SerializeField] private float radius = 0.5f;

        [Header("Audio")]
        [SerializeField] private AudioSource start;
        [SerializeField] private AudioSource complete;
        [SerializeField] private Explosion explosion;

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
        protected override void StartChannel(Perceivable self, Perceivable target)
        {
            //Base
            base.StartChannel(self, target);

            //Play audio
            if (start != null) start.Play();

            //Destroy corpse
            Destroy(target.gameObject);
        }
        protected override void CompleteChannel(Perceivable self, Perceivable target)
        {
            //Self required
            if (self == null) return;

            //Grab formation spawner
            FormationSpawn spawn = self.GetComponentInParent<FormationSpawn>();

            //Insantiate units
            if (count == 1) SummonUnit(summon, transform.position, self.transform.up, spawn, self.Faction);
            else
            {
                float summonSegment = 360.0f / count;
                for (int i = 0; i < count; i++)
                {
                    float angle = summonSegment * i;
                    Vector3 positionOffset = Quaternion.Euler(0, 0, angle) * Vector3.up * radius;
                    SummonUnit(summon, transform.position + positionOffset, positionOffset.normalized, spawn, self.Faction);
                }
            }

            //Play audio
            if (start != null) complete.Play();

            //Trigger explostion
            explosion.Trigger();
        }
    }
}