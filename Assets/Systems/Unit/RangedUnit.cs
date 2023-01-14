using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Pooling;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(AudioSource))]
    public class RangedUnit : BaseUnit
    {
        [Header("Ranged Attack")]
        [SerializeField] private Transform barrel;
        [SerializeField] private Projectile projectile;
        [SerializeField] private float projectileSpeed = 40.0f;
        [SerializeField] private float attackRange = 4.0f;
        [SerializeField] private float recovery = 0.1f;

        //Components
        private Animator anim;
        private AudioSource audioSource;

        //Targeting
        private Vector3 attackDirection;

        //Mono
        protected override void OnEnable()
        {
            base.OnEnable();

            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        //Action
        protected override void Targeting()
        {
            actionTarget = perceiver.GetNearestVisibleTarget();
            movementTarget = actionTarget;
        }

        protected override void SetupAction(float deltaTime)
        {
            //Target required
            if (actionTarget == null) return;

            //Calculate distance to target
            float distance = Vector3.Distance(actionTarget.transform.position, transform.position);

            //Setup attack for next frame when available
            if (distance < attackRange && projectile != null && perceiver.UnitVisible(actionTarget, true))
            {
                //Reduce targeting time
                targetTime -= deltaTime;

                //Attack when targeting complete
                if (targetTime <= 0)
                {
                    //Now attacking
                    state = UnitState.Actioning;

                    //Calculate direction
                    attackDirection = (actionTarget.transform.position - transform.position).normalized;
                }
            }
        }
        protected override void Action(float deltaTime)
        {
            //Projectile required
            if (projectile == null) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            //Instantiate projectile
            Projectile proj = PoolManager.RequestPoolable(projectile, barrel.position, Quaternion.LookRotation(Vector3.forward, attackDirection)) as Projectile;

            //Add force
            proj.Launch(this, perceiver.Faction, attackDirection * projectileSpeed);

            //Play hit
            audioSource.Play();

            //Play animation
            if (anim != null) anim.Play("Attack");

            //Now recovering
            Hold(recovery);
        }
    }
}