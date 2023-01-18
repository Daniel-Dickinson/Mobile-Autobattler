using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;
using TwoBears.Pooling;

namespace TwoBears.Unit
{
    public abstract class ChannelAbility : Ability
    {
        [Header("Channeling")]
        [SerializeField] private float channelTime = 0.8f;

        [Header("Effect")]
        [SerializeField] private AOESpellEffect effect;
        [SerializeField] private AOEEffectMode mode;

        //Event Access
        public Action OnComplete;
        public Action OnInterrupt;

        //Access
        public override void Trigger(Perceivable self, Perceivable target)
        {
            StartCoroutine(Channel(self, target));
        }
        public void Interrupt()
        {
            //Stop all routines
            StopAllCoroutines();

            //Interrupt
            InterruptChannel();
        }

        //Routine
        private IEnumerator Channel(Perceivable self, Perceivable target)
        {
            UpdateEffect(0.0f);
            StartChannel(self, target);

            float timeElapsed = 0.0f;
            while (timeElapsed < channelTime)
            {
                //Increment time
                timeElapsed += Time.deltaTime;

                //Calculate lerp
                float lerp = Mathf.Clamp01(timeElapsed / channelTime);
                
                //Update effect
                UpdateEffect(lerp);

                //Next frame
                yield return new WaitForEndOfFrame();
            }

            CompleteChannel(self, target);
            UpdateEffect(1.0f);

            //Next frame
            yield return new WaitForEndOfFrame();

            //Cleanup
            PostChannel(self, target);
        }

        //Functionality
        protected virtual void StartChannel(Perceivable self, Perceivable target)
        {

        }
        protected abstract void CompleteChannel(Perceivable self, Perceivable target);
        protected virtual void PostChannel(Perceivable self, Perceivable target)
        {
            //Interrupt event
            OnComplete?.Invoke();

            //Return
            PoolManager.ReturnPoolable(this);
        }

        protected virtual void InterruptChannel()
        {
            //Interrupt event
            OnInterrupt?.Invoke();

            //Return
            PoolManager.ReturnPoolable(this);
        }

        //Effect
        private void UpdateEffect(float lerp)
        {
            if (effect == null) return;

            switch (mode)
            {
                case AOEEffectMode.Inwards:
                    effect.SetIndicator(lerp);
                    break;

                case AOEEffectMode.Outwards:
                    effect.SetIndicator(1 - lerp);
                    break;
            }
        }
    }

    public enum AOEEffectMode { Inwards, Outwards }
}