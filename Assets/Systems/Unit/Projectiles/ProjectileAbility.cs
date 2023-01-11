using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Perception;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Projectile))]
    public abstract class ProjectileAbility : MonoBehaviour
    {
        private Projectile projectile;

        //Mono
        private void Awake()
        {
            projectile = GetComponent<Projectile>();
            projectile.OnHit += Trigger;
        }
        private void OnDestroy()
        {
            projectile.OnHit -= Trigger;
        }

        //Effect
        public abstract void Trigger(BaseUnit launcher, Faction faction, Collision2D collision);
    }
}