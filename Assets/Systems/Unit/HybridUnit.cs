using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class HybridUnit : BaseUnit
    {
        [Header("Melee Attack")]
        [SerializeField] private float chargeRange = 0.4f;
        [SerializeField] private float chargeSpeed = 2.5f;
        [SerializeField] private float strikeRange = 0.2f;
        [SerializeField] private float strikeRecovery = 1.0f;

        [Header("Ranged Attack")]
        [SerializeField] private Transform barrel;
        [SerializeField] private Projectile projectile;
        [SerializeField] private float projectileSpeed = 40.0f;
        [SerializeField] private float projectileRange = 4.0f;
        [SerializeField] private float projectileRecovery = 0.1f;

        //Components
        private Animator anim;
        private AudioSource audioSource;

        //Weapons
        protected Weapon weapon;

        //Action
        private HybridActionType type;

        //Melee Action
        private Vector3 attackPosition;
        private float attackTime;

        //Ranged Action
        private Vector3 attackDirection;

        //Mono
        protected override void OnEnable()
        {
            base.OnEnable();

            //Grab weapon
            weapon = GetComponent<Weapon>();

            //Grab anim & audio
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

            //Setup approproate action
            if (distance > chargeRange)
            {
                SetupRangedAction(distance, deltaTime);
            }
            else
            {
                SetupMeleeAction(distance, deltaTime);
            }
        }
        protected override void Action(float deltaTime)
        {
            switch (type)
            {
                case HybridActionType.Melee:
                    MeleeAction(deltaTime);
                    break;
                case HybridActionType.Ranged:
                    RangedAction(deltaTime);
                    break;
            }
        }

        //Melee
        protected void SetupMeleeAction(float distance, float deltaTime)
        {
            //Setup attack for next frame when available
            if (distance < (chargeRange + 0.05f) && weapon != null && weapon.enabled && perceiver.UnitVisible(actionTarget))
            {
                //Reduce targeting time
                targetTime -= deltaTime;

                //Attack when targeting complete
                if (targetTime <= 0)
                {
                    //Now attacking
                    state = UnitState.Actioning;

                    //Set attack type
                    type = HybridActionType.Melee;

                    //Calculate direction
                    Vector3 direction = (actionTarget.transform.position - transform.position).normalized;

                    //Set attack position & time
                    attackPosition = actionTarget.transform.position + (direction * 0.15f);
                    attackTime = distance / chargeSpeed;
                }
            }
        }
        protected void MeleeAction(float deltaTime)
        {
            //Weapon required
            if (weapon == null && weapon.enabled) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            //Calculate setup data
            float distance = Vector3.Distance(attackPosition, transform.position);
            Vector3 direction = (attackPosition - transform.position).normalized;
            Vector3 goalPosition = attackPosition - (direction * strikeRange);

            //Charge towards attack position
            rb.MovePosition(Vector3.MoveTowards(transform.position, goalPosition, chargeSpeed * deltaTime));

            //Rotate towards attack position
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            //Increment attack time (Fail safe in case attack position is unreachable)
            attackTime -= deltaTime;

            //Attack when within range
            if (attackTime <= 0 || distance < strikeRange + 0.05f)
            {
                //Strike
                weapon.Strike();

                //Now recovering
                Hold(strikeRecovery);
            }
        }

        //Ranged
        protected void SetupRangedAction(float distance, float deltaTime)
        {
            //Setup attack for next frame when available
            if (distance < projectileRange && projectile != null && perceiver.UnitVisible(actionTarget, true))
            {
                //Reduce targeting time
                targetTime -= deltaTime;

                //Attack when targeting complete
                if (targetTime <= 0)
                {
                    //Now attacking
                    state = UnitState.Actioning;

                    //Set attack type
                    type = HybridActionType.Ranged;

                    //Calculate direction
                    attackDirection = (actionTarget.transform.position - transform.position).normalized;
                }
            }
        }
        protected void RangedAction(float deltaTime)
        {
            //Projectile required
            if (projectile == null) return;

            //Must still be attacking
            if (state != UnitState.Actioning) return;

            //Instantiate projectile
            Projectile proj = Instantiate(projectile, barrel.position, Quaternion.LookRotation(Vector3.forward, attackDirection));

            //Add force
            proj.Launch(this, perceiver.Faction, attackDirection * projectileSpeed);

            //Play hit
            audioSource.Play();

            //Play animation
            if (anim != null) anim.Play("Attack");

            //Now recovering
            Hold(projectileRecovery);
        }

        //Death
        protected override void Kill()
        {
            //Base
            base.Kill();

            //Disable weapon
            if (weapon != null) weapon.enabled = false;
        }
    }

    public enum HybridActionType { Melee, Ranged }
}