using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Pooling
{
    public class Poolable : MonoBehaviour
    {
        [Header("Identification")]
        [SerializeField] private string poolID = "Default Poolable";

        [Header("Lifetime")]
        [SerializeField] protected float lifetime = 0;

        public Action OnSpawn;
        public Action OnUpdate;
        public Action OnReset;

        //Lifetime
        protected float lifeRemaining;

        //Access
        public string PoolID
        {
            get { return poolID; }
        }

        //Core
        public virtual void PoolableInit()
        {
            //Set initial life
            lifeRemaining = lifetime;

            //On spawn
            OnSpawn?.Invoke();
        }
        public virtual void PoolableUpdate(float deltaTime)
        {
            //Track lifetime
            if (lifetime > 0)
            {
                lifeRemaining -= deltaTime;
                if (lifeRemaining <= 0) Return();
            }
        }
        public virtual void PoolableReset()
        {
            //On reset
            OnReset?.Invoke();
        }

        //Return
        public virtual void Return()
        {
            PoolManager.ReturnPoolable(this);
        }
        
    }
}