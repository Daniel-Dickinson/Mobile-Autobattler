using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Pooling;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Poolable))]
    public class HealEffect : MonoBehaviour
    {
        [Header("Effects")]
        public ParticleSystem effect1;
        public ParticleSystem effect2;
        public ParticleSystem effect3;
        public ParticleSystem effect4;
        public ParticleSystem effect5;

        //Components
        Poolable poolable;

        //Mono
        private void Awake()
        {
            poolable = GetComponent<Poolable>();
            poolable.OnReset += PoolableReset;

            //Set all as inactive by default
            PoolableReset();
        }
        private void OnDestroy()
        {
            poolable.OnReset -= PoolableReset;
        }

        //Core
        public void ActivateParticles(int healing)
        {
            switch (healing)
            {
                default:
                case 1:
                    effect1.gameObject.SetActive(true);
                    break;
                case 2:
                    effect2.gameObject.SetActive(true);
                    break;
                case 3:
                case 4:
                    effect3.gameObject.SetActive(true);
                    break;
                case 5:
                case 6:
                    effect4.gameObject.SetActive(true);
                    break;
                case > 6:
                    effect5.gameObject.SetActive(true);
                    break;
            }
        }
        private void PoolableReset()
        {
            effect1.gameObject.SetActive(false);
            effect2.gameObject.SetActive(false);
            effect3.gameObject.SetActive(false);
            effect4.gameObject.SetActive(false);
            effect5.gameObject.SetActive(false);
        }
    }
}
