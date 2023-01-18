using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using UnityEditor;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(AOESpellEffect))]
    public abstract class PassiveAOE : MonoBehaviour
    {
        [Header("Metrics")]
        public float period = 3.0f;
        public float radius = 1.0f;
        
        //Timing
        private float timeElapsed = 0;

        //Components
        private AOESpellEffect indicator;
        private BaseUnit unit;

        //Mono
        private void Awake()
        {
            //Grab indicator
            indicator = GetComponent<AOESpellEffect>();
            unit = GetComponentInParent<BaseUnit>();
        }
        private void Update()
        {
            Build(Time.deltaTime);
        }

        //Access
        private void Build(float deltaTime)
        {
            //Increment time elapsed
            timeElapsed += deltaTime;

            //Calculate lerp
            float lerp = Mathf.Clamp01(timeElapsed / period);
            indicator.SetIndicator(lerp);

            //Trigger when appropriate
            if (timeElapsed > period) Trigger(unit);
        }
        private void Trigger(BaseUnit self)
        {
            //Reset time elapsed
            timeElapsed = 0;

            //Grab perceivables
            List<Perceivable> targets = Perceivable.Perceivables;

            //Find next target
            for (int i = 0; i < targets.Count; i++)
            {
                //Check valid
                if (targets[i] == null) continue;

                //Calculate distance
                Vector3 vector = targets[i].transform.position - transform.position;
                float distance = vector.magnitude;

                //Distance check
                if (distance > radius) continue;

                //Grab base unit
                BaseUnit unit = targets[i].GetComponent<BaseUnit>();

                //Custom targeting - Defined per passive
                if (!IsValidTarget(self, unit)) continue;

                //Apply effect
                ApplyEffect(unit);
                return;
            }
        }

        //Targeting
        protected abstract bool IsValidTarget(BaseUnit self, BaseUnit unit);

        //Effect
        protected abstract void ApplyEffect(BaseUnit unit);

        #if UNITY_EDITOR
        //Gizmos
        private void OnDrawGizmosSelected()
        {
            Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
        }
        #endif
    }
}