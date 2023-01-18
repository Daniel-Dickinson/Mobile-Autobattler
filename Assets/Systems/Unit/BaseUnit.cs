using UnityEngine;

using Pathfinding;
using TwoBears.Pooling;
using TwoBears.Perception;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(Perceiver))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class BaseUnit : MonoBehaviour
    {
        [Header("Class")]
        public UnitClass primary;
        public UnitClass secondary;
        public UnitClass tertiary;

        [Header("Health")]
        [SerializeField] private int healthPool = 3;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private float moveRangeMin = 0.5f;
        [SerializeField] private float moveRangeMax = 0.5f;

        [Header("Circling")]
        [SerializeField] private CircleMode circleMode;
        [SerializeField] private float circleRange = 3.0f;
        [SerializeField] private float circleSpeed = 1.0f;

        [Header("Personality")]
        public float hesitanceMin = 0.5f;
        public float hesitanceMax = 1.0f;

        [Header("Particles")]
        public Poolable healEffect;
        public Poolable hitEffect;
        public float hitRadius = 0.2f;

        [Header("Subshapes")]
        public GameObject[] shapes;

        [Header("Debug")]
        [SerializeField] private bool debugGoal = false;
        [SerializeField] private bool debugCrowd = false;

        //Access
        public Rigidbody2D RigidBody
        {
            get { return rb; }
        }

        //Health
        public int MaxHealth
        {
            get { return healthPool; }
        }
        public int Health
        { 
            get { return health; } 
        }
        private int health = 1;

        //State
        protected UnitState state;

        //Events
        public UnitHealthEvent OnDamaged;
        public UnitHealthEvent OnHealed;
        public UnitEvent OnDeath;

        //Components
        protected Perceiver perceiver;
        protected Rigidbody2D rb;
        protected Collider2D col;

        //Targeting
        protected Perceivable actionTarget;
        protected Perceivable movementTarget;
        protected float targetTime;

        //Pathfinding
        private Seeker seeker;
        private Vector3 pathGoal;
        private int pathIndex;
        private Path path;

        //AntiCrowding
        private Vector2 antiCrowd;

        //Recovery
        private float recoveryTime;

        //Buffs
        public virtual float MoveSpeedIncrease
        {
            get { return moveSpeedIncrease; }
            set { moveSpeedIncrease = value; }
        }
        private float moveSpeedIncrease = 0;
        
        public virtual float DamageMultiplier
        {
            get { return damageMultiplier; }
            set
            {
                damageMultiplier = value;
            }
        }
        protected float damageMultiplier = 1.0f;

        //Tuning
        private const float globalMoveSpeedModifier = 0.06f;

        //Mono
        protected virtual void OnEnable()
        {
            //Set starting health
            health = healthPool;

            //Grab perceiver
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            perceiver = GetComponent<Perceiver>();
            seeker = GetComponent<Seeker>();

            //Randomize build time
            targetTime = Random.Range(hesitanceMin, hesitanceMax);

            //Enable sub-shape
            if (shapes != null) foreach (GameObject shape in shapes) shape.SetActive(true);
        }
        protected virtual void OnDisable()
        {
           //Disable sub-shape
            if (shapes != null) foreach (GameObject shape in shapes) shape.SetActive(false);

            //Deregister
            Deregister();
        }
        protected virtual void OnDestroy()
        {
            //Kill -- Give other systems a chance to detach events
            OnDeath?.Invoke(this);
        }
        protected void FixedUpdate()
        {
            PerformBehaviour(Time.fixedDeltaTime);
        }

        //Registration -- Into Unit Manager
        public void Initialize()
        {
            //Set faction before registration
            //Must be called manually after faction is set

            UnitManager.RegisterUnit(this);
        }
        private void Deregister()
        {
            UnitManager.DeregisterUnit(this);
        }

        //Operations -- Performed by Unit Manager
        public bool TargetingRequired
        {
            get { return state == UnitState.Movement; }
        }
        public bool PathfindingRequired
        {
            get { return state == UnitState.Movement; }
        }
        public bool AntiCrowdingRequired
        {
            get { return state == UnitState.Movement; }
        }

        public void UpdateObstacles()
        {
            //Update perceiver obstacles
            perceiver.UpdateObstacles();
        }
        public void UpdateTargeting()
        {
            //Update targeting
            Targeting();
        }
        public void UpdatePathfinding()
        {
            //Calculate path
            Pathfinding();
        }
        public void UpdateAntiCrowding()
        {
            AntiCrowding();
        }

        //Buffing
        public virtual void RaiseMaxHealth(int delta)
        {
            healthPool += delta;
            health += delta;
        }
        public virtual void IncreaseRange(float amount)
        {
            //Increase max move range
            moveRangeMax *= (1.0f + amount);
        }

        //Behaviour
        private void PerformBehaviour(float deltaTime)
        {
            switch (state)
            {
                case UnitState.Movement:

                    //Target required
                    if (actionTarget == null) return;

                    //Setup action for next frame
                    SetupAction(deltaTime);
                    
                    //Move as normal
                    Move(deltaTime);
                    break;

                case UnitState.Actioning:
                    Action(deltaTime);
                    break;

                case UnitState.Recovering:
                    Recover(deltaTime);
                    break;
            }
        }

        //Targeting
        protected virtual void Targeting()
        {
            //Get nearest target
            actionTarget = perceiver.GetNearestTarget();

            //Movement target same as action
            movementTarget = actionTarget;
        }

        //Movement
        protected float MovementRange(float distanceToTarget)
        {
            return Mathf.Clamp(distanceToTarget, moveRangeMin, moveRangeMax);
        }
        protected virtual void Move(float deltaTime)
        {
            //Calculate move speed
            float speed = (moveSpeed + moveSpeedIncrease) * globalMoveSpeedModifier;

            //Calculate path position
            Vector2 goalPosition = TraversePath(movementTarget.transform.position, speed);
            Vector3 goalVector = movementTarget.transform.position - transform.position;
            Vector3 direction = goalVector.normalized;
            float distance = goalVector.magnitude;

            //Switch between movement behaviours -- AntiCrowd returns very different between modes
            if (distance <= circleRange)
            {
                //Calculate ideal surround distance
                float surroundDistance = MovementRange(distance);

                //Circle & avoid obstacles as we get close
                if (antiCrowd.x != 0) direction = Quaternion.Euler(0, 0, -antiCrowd.x) * direction;

                //Don't get closer than movement range
                if ((path != null && pathIndex >= path.path.Count - 1) || distance <= surroundDistance)
                {
                    if (antiCrowd.y >= 0)
                    {
                        //Charge forward by default
                        goalPosition = movementTarget.transform.position - (direction * surroundDistance);
                    }
                    else
                    {
                        //Move backwards if crowded
                        goalPosition = transform.position + (antiCrowd.y * direction * circleSpeed);
                    }
                }
            }
            else
            {
                //Localize goal then add anticrowding
                goalPosition = rb.position + ((goalPosition - rb.position).normalized * speed);
                goalPosition += antiCrowd;
            }

            //Set velocity to move
            rb.velocity = (goalPosition - rb.position).normalized * speed;

            //Debug move position
            if (debugGoal)
            {
                Debug.DrawLine(goalPosition + new Vector2(-0.1f, 0), goalPosition + new Vector2(0.1f, 0), Color.green);
                Debug.DrawLine(goalPosition + new Vector2(0, -0.1f), goalPosition + new Vector2(0, 0.1f), Color.green);
            }

            //Rotate
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, (actionTarget.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 600.0f * deltaTime);
        }
        protected virtual void Recover(float deltaTime)
        {
            if (recoveryTime > 0) recoveryTime -= deltaTime;

            //Rotate
            if (actionTarget != null)
            {
                Quaternion rotation = Quaternion.LookRotation(Vector3.forward, (actionTarget.transform.position - transform.position).normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 300.0f * deltaTime);
            }

            //Return to movement after recovery
            if (recoveryTime <= 0 && rb.velocity.magnitude <= 0.05f) state = UnitState.Movement;
        }

        //Action
        protected abstract void SetupAction(float deltaTime);
        protected abstract void Action(float deltaTime);

        //Pathfinding
        private void Pathfinding()
        {
            //Path towards movement target -- fallback self so still valid path
            Vector3 goalPosition = (movementTarget != null)? movementTarget.transform.position : transform.position;

            //Check path status
            if (path == null || seeker.IsDone())
            {
                //Update path as required
                if (path == null || path != seeker.GetCurrentPath())
                {
                    path = seeker.GetCurrentPath();
                    pathIndex = 0;
                }

                //Recalculate as needed
                if (path == null || Vector3.Distance(pathGoal, goalPosition) > 0.1f)
                {
                    pathGoal = goalPosition;
                    seeker.StartPath(transform.position, pathGoal);
                }
            }
        }
        private Vector3 TraversePath(Vector3 goalPosition, float speed = 0.5f)
        {
            //Valid path required
            if (path == null) return transform.position;
            if (path.path == null || path.path.Count == 0) return transform.position;

            //Move directly to goal if at last position
            if (pathIndex >= path.path.Count - 1) return goalPosition;

            //Get next position
            Vector3 nextPosition = (Vector3)path.path[pathIndex].position;

            //If we've reached the position get increment
            while (Vector3.Distance(transform.position, nextPosition) < speed)
            {
                pathIndex++;

                //Move to goal if at last position
                if (pathIndex >= path.path.Count - 1) return goalPosition;

                //Next position
                nextPosition = (Vector3)path.path[pathIndex].position;
            }

            //Return position of next node
            return nextPosition;
        }

        //Anticrowding
        private void AntiCrowding()
        {
            //Movement target required
            if (movementTarget == null)
            {
                antiCrowd = Vector2.zero;
                return;
            }

            //Calculate path position
            Vector3 goalVector = movementTarget.transform.position - transform.position;
            Vector3 direction = goalVector.normalized;
            float distance = goalVector.magnitude;

            //Perform
            antiCrowd = AntiCrowding(distance, direction);
        }
        private Vector2 AntiCrowding(float distance, Vector3 direction)
        {
            if (distance <= circleRange) return movementTarget.CalculateApproachDirection(perceiver, direction, distance, circleMode, debugCrowd) * circleSpeed;
            else return perceiver.NearestAntiCrowding(movementTarget);
        }

        //Health
        public void RemoveHealth(int damage)
        {
            //Can only lower to zero
            int delta = Mathf.Min(health, damage);

            //Damage
            health -= delta;
            OnDamaged?.Invoke(delta);

            //Kill if zero
            if (health <= 0) Kill();
        }
        public void RestoreHealth(int amount)
        {
            //Can only raise to max health
            int delta = Mathf.Min(amount, healthPool - health);
            if (delta == 0) return;

            //Play particles
            TriggerHealParticles(delta);

            //Increase health
            health = Mathf.Clamp(health + delta, 0, healthPool);

            //Heal event
            OnHealed?.Invoke(delta);
        }

        //Particles
        public void TriggerHealParticles(int healing)
        {
            //Request particle system
            Poolable heal = PoolManager.RequestPoolable(healEffect, transform.position, Quaternion.LookRotation(Vector3.forward, transform.up), transform);

            //Activate particles
            heal.GetComponent<HealEffect>().ActivateParticles(healing);
        }
        public void TriggerHitParticles(Vector3 direction, int damage)
        {
            //Request particle system
            Poolable hit = PoolManager.RequestPoolable(hitEffect, transform.position + (direction * hitRadius), Quaternion.LookRotation(Vector3.forward, direction));

            //Activate particles
            hit.GetComponent<HitEffect>().ActivateParticles(damage);
        }

        //Recovering
        protected void Hold(float duration)
        {
            state = UnitState.Recovering;

            //Wait for duration
            recoveryTime = duration;

            //Randomize build time
            targetTime = Random.Range(hesitanceMin, hesitanceMax);
        }
        public virtual void KnockBack(Vector2 knockback)
        {
            state = UnitState.Recovering;

            //Add knockback force
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            rb.AddForce(knockback, ForceMode2D.Impulse);

            //Add minimaal recovery time
            //Prevents instant recovery before velocity has time to apply
            recoveryTime = 0.1f;

            //Randomize build time
            targetTime = Random.Range(hesitanceMin, hesitanceMax);
        }

        //Death
        protected virtual void Kill()
        {
            //On death
            OnDeath?.Invoke(this);

            //Disable components
            enabled = false;
            seeker.enabled = false;

            //Convert to corpse
            perceiver.ConvertToCorpse();
        }
    }

    public enum UnitState { Movement, Actioning, Recovering }
    public enum UnitClass { None, Warrior, Defender, Ranger, Healer, Caster, Merchant, Summoner, Minion }

    public delegate void UnitEvent(BaseUnit unit);
    public delegate void UnitHealthEvent(int delta);
}