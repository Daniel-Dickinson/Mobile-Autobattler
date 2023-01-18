using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Cameras;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(TrailRenderer))]
    [RequireComponent(typeof(CameraShaker))]
    public abstract class ChainAbility : Ability
    {
        [Header("Chaining")]
        [SerializeField] private int chainLength = 3;
        [SerializeField] private float chainDistance = 1.0f;
        [SerializeField] private float chainSpeed = 10.0f;
        [SerializeField] private float chainDelay = 0.1f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        //Buff Access
        public int ChainIncrease
        {
            set { chainIncrease = value; }
        }
        public float DistanceMultiplier
        {
            get { return distanceMultiplier; }
            set { distanceMultiplier = value; }
        }

        //Buffing
        private int ChainLength
        {
            get { return chainLength + chainIncrease; }
        }
        private int chainIncrease = 0;

        private float ChainDistance
        {
            get { return chainDistance * distanceMultiplier; }
        }
        private float distanceMultiplier = 1.0f;

        //Chaining
        private int chains = 0;
        private List<BaseUnit> units;

        //Unit
        private Faction faction;

        //Components
        private TrailRenderer trail;
        private CameraShaker shaker;

        //Mono
        private void Awake()
        {
            //Grab trail
            trail = GetComponent<TrailRenderer>();
            shaker = GetComponent<CameraShaker>();
        }

        //Core
        public override void Trigger(Perceivable self, Perceivable target)
        {
            //Grab unit
            BaseUnit unit = target.GetComponent<BaseUnit>();

            //Zero out
            chains = 0;

            //Initialize units
            if (units == null) units = new List<BaseUnit>();
            else units.Clear();

            //Cache faction
            faction = target.Faction;

            //Jump to next target
            StartCoroutine(JumpToUnit(unit));
        }

        //Poolable
        public override void PoolableInit()
        {
            base.PoolableInit();

            //Clear trail
            trail.Clear();
        }
        public override void PoolableReset()
        {
            base.PoolableReset();

            //Reset
            chains = 0;

            //Clear units
            units.Clear();

            //Clear trail
            trail.Clear();
        }

        //Trigger
        protected abstract void TriggerChainEffect(BaseUnit unit, Vector3 direction);

        //Targeting
        protected virtual bool IsValidChainTarget(BaseUnit unit)
        {
            return true;
        }

        //Chaining
        private IEnumerator JumpToUnit(BaseUnit unit)
        {
            //Enable trail
            trail.emitting = true;

            //Calculate direction
            Vector3 vector = unit.transform.position - transform.position;

            while (Vector3.Distance(transform.position, unit.transform.position) > 0.05f)
            {
                //Move towards target unit
                transform.position = Vector3.MoveTowards(transform.position, unit.transform.position, chainSpeed * Time.deltaTime);

                //Next frame
                yield return new WaitForEndOfFrame();
            }

            //Disable trail
            trail.emitting = false;

            //Trigger
            TriggerChainInstance(unit, vector.normalized);

            //Wait chain delay
            float timePassed = 0;
            while (timePassed < chainDelay)
            {
                //Track time passed
                timePassed += Time.deltaTime;

                //Move with target unit
                transform.position = Vector3.MoveTowards(transform.position, unit.transform.position, chainSpeed * Time.deltaTime);

                //Next frame
                yield return new WaitForEndOfFrame();
            }

            //Get next target
            GetNextValidTarget();
        }
        private void TriggerChainInstance(BaseUnit unit, Vector3 direction)
        {
            //Play audio
            if (audioSource != null) audioSource.Play();

            //Shake camera
            shaker.Trigger();

            //Trigger effect
            TriggerChainEffect(unit, direction);

            //Increment chains
            chains++;

            //Add to chained units -- excludes from future chains
            units.Add(unit);

            //Reset life
            lifeRemaining = lifetime;
        }
        private void GetNextValidTarget()
        {
            //Check against chain length
            if (chains >= ChainLength) return;

            //Grab perceivables
            List<Perceivable> targets = Perceivable.Perceivables;

            //Find next target
            for (int i = 0; i < targets.Count; i++)
            {
                //Check faction
                if (targets[i] == null) continue;
                if (targets[i].Faction != faction) continue;

                //Calculate distance
                Vector3 vector = targets[i].transform.position - transform.position;
                float distance = vector.magnitude;

                //Distance check
                if (distance > ChainDistance) continue;

                //Grab base unit
                BaseUnit unit = targets[i].GetComponent<BaseUnit>();

                //Custom targeting - Defined per ability
                if (!IsValidChainTarget(unit)) continue;

                //Check if already targeted
                if (units.Contains(unit)) continue;

                //Jump to next target
                StartCoroutine(JumpToUnit(unit));
                return;
            }

            //No target found - disable trail
            trail.emitting = false;
        }
    }
}