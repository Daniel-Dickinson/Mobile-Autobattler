using UnityEngine;

using Pathfinding;
using TwoBears.Perception;

namespace TwoBears.Unit
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(Perceiver))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class BaseUnit : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int healthPool = 3;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 0.5f;
        
        [SerializeField] private float moveSmooth = 0.01f;

        [Header("Circling")]
        [SerializeField] private float circleRange = 3.0f;
        [SerializeField] private float circleSpeed = 1.0f;

        [Header("Personality")]
        public float hesitanceMin = 0.5f;
        public float hesitanceMax = 1.0f;

        [Header("Subshape")]
        public GameObject shape;

        [Header("Debug")]
        [SerializeField] private bool debugGoal = false;
        [SerializeField] private bool debugCrowd = false;

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
        public UnitEvent OnDamaged;
        public UnitEvent OnHealed;
        public UnitEvent OnDeath;

        //Access
        public Rigidbody2D RigidBody
        {
            get { return rb; }
        }

        //Components
        protected Perceiver perceiver;
        protected Rigidbody2D rb;
        protected Collider2D col;

        //Targeting
        protected Perceivable target;
        protected float targetTime;

        //Pathfinding
        private Seeker seeker;
        private Vector3 pathGoal;
        private int pathIndex;
        private Path path;

        //Movement
        private Vector3 moveVelocity;

        //Recovery
        private float recoveryTime;        

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
            targetTime = Random.Range(0, hesitanceMax);

            //Enable sub-shape
            if (shape != null) shape.SetActive(true);
        }
        protected virtual void OnDisable()
        {
            //Disable sub-shape
            if (shape != null) shape.SetActive(false);
        }
        protected virtual void FixedUpdate()
        {
            PerformBehaviour(Time.fixedDeltaTime);
        }

        //Behaviour
        private void PerformBehaviour(float deltaTime)
        {
            //Get nearest target
            target = perceiver.GetTarget();

            //Target required
            if (target == null) return;

            switch (state)
            {
                case UnitState.Movement:

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
        protected virtual void Move(float deltaTime)
        {
            //Calculate path to goal position
            UpdatePath(target.transform.position);

            //Calculate path position
            Vector3 goalPosition = TraversePath(target.transform.position);
            Vector3 goalVector = target.transform.position - transform.position;
            Vector3 direction = goalVector.normalized;
            float distance = goalVector.magnitude;

            //Circle & avoid obstacles as we get close
            Vector2 circleDir = (distance <= circleRange)? target.CalculateApproachDirection(perceiver, direction, debugCrowd) * circleSpeed : Vector2.zero;
            if (circleDir.x != 0) direction = Quaternion.Euler(0, 0, -circleDir.x) * direction;

            //Don't get closer than attack range
            if ((path != null && pathIndex >= path.path.Count - 1) || Vector3.Distance(goalPosition, target.transform.position) <= ActionRange(distance))
            {
                //Charge forward by default
                goalPosition = target.transform.position - (direction * ActionRange(distance));

                //Move backwards if crowded
                if (circleDir.y < 0) goalPosition = transform.position - (direction * circleSpeed);
            }

            //Move
            Vector3 movePosition = Vector3.MoveTowards(transform.position, goalPosition, moveSpeed * deltaTime);
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, movePosition, ref moveVelocity, moveSmooth);

            rb.MovePosition(smoothedPosition);
            
            //Debug move position
            if (debugGoal)
            {
                Debug.DrawLine(goalPosition + new Vector3(-0.1f, 0, 0), goalPosition + new Vector3(0.1f, 0, 0), Color.green);
                Debug.DrawLine(goalPosition + new Vector3(0, -0.1f, 0), goalPosition + new Vector3(0, 0.1f, 0), Color.green);
            }

            //Rotate
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, (target.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 180.0f * deltaTime);
        }
        protected virtual void Recover(float deltaTime)
        {
            if (recoveryTime > 0) recoveryTime -= deltaTime;

            //Return to movement after recovery
            if (recoveryTime <= 0 && rb.velocity.magnitude <= 0.05f) state = UnitState.Movement;
        }

        //Action
        protected abstract float ActionRange(float distanceToTarget);
        protected abstract void SetupAction(float deltaTime);
        protected abstract void Action(float deltaTime);

        //Pathfinding
        private void UpdatePath(Vector3 goalPosition)
        {
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
        private Vector3 TraversePath(Vector3 goalPosition)
        {
            if (path == null) return goalPosition;

            //Get next position
            Vector3 nextPosition = (Vector3)path.path[pathIndex].position;

            //Move to goal if at last position
            if (pathIndex >= path.path.Count - 1) return goalPosition;

            //If we've reached the position get increment
            if (Vector3.Distance(transform.position, nextPosition) < 0.5f) pathIndex++;

            //Return position of next node
            return nextPosition;
        }

        //Health
        public void RemoveHealth(int damage)
        {
            //Damage
            health -= damage;
            OnDamaged?.Invoke(this);

            if (health <= 0) Kill();
        }
        public void RestoreHealth(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, healthPool);
            OnHealed?.Invoke(this);
        }

        //Recovering
        protected void Hold(float duration)
        {
            state = UnitState.Recovering;

            //Wait for duration
            recoveryTime = duration;

            //Randomize build time
            targetTime = UnityEngine.Random.Range(hesitanceMin, hesitanceMax);
        }
        public void KnockBack(Vector2 knockback)
        {
            state = UnitState.Recovering;

            //Add knockback force
            rb.AddForce(knockback, ForceMode2D.Impulse);

            //Randomize build time
            targetTime = UnityEngine.Random.Range(hesitanceMin, hesitanceMax);
        }

        //Death
        protected virtual void Kill()
        {
            //On death
            OnDeath?.Invoke(this);

            //Disable components
            enabled = false;
            seeker.enabled = false;
            perceiver.enabled = false;

            //Disable physics
            col.enabled = false;
        }
    }

    public enum UnitState { Movement, Actioning, Recovering }
    public delegate void UnitEvent(BaseUnit unit);
}