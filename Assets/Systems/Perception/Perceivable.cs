using System.Collections.Generic;
using TwoBears.Unit;
using UnityEngine;

namespace TwoBears.Perception
{
    public class Perceivable : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private Faction faction;

        [Header("Units")]
        public float radius = 0.5f;
        public LayerMask unitMask;

        [Header("Anti-Crowding")]
        public float crowdingRadius = 1.0f;
        public float crowdingSpread = 0.4f;

        [Header("Obstacles")]
        public int rayCount = 32;
        public float nearDistance = 2.0f;
        public float farDistance = 6.0f;
        public LayerMask obstacleMask;

        [Header("Death")]
        public bool producesCorpse = true;
        public int corpseLayer = 9;

        [Header("Debug")]
        [SerializeField] protected bool debugDirection = false; 
        [SerializeField] protected bool debugObstacles = false;

        //Obstacles
        private ObstacleSegment[] obstacleSegments;

        //Data
        protected static List<Perceivable> perceivables;
        protected static List<Perceivable> corpses;

        //Access
        public Faction Faction
        {
            get { return faction; }
            set { faction = value; }
        }
        public bool IsHostileTowards(Faction faction)
        {
            if (this.faction == faction) return false;
            else return true;
        }

        //Mono
        private void OnEnable()
        {
            Register();

            //Create obstacle segments
            obstacleSegments = new ObstacleSegment[rayCount];
            for (int i = 0; i < rayCount; i++) obstacleSegments[i] = new ObstacleSegment(i, rayCount);
        }
        private void OnDisable()
        {
            Deregister();
        }

        //Registration
        private void Register()
        {
            if (perceivables == null) perceivables = new List<Perceivable>();
            perceivables.Add(this);
        }
        private void Deregister()
        {
            if (perceivables == null) return;
            perceivables.Remove(this);

            if (corpses == null) return;
            corpses.Remove(this);
        }

        //Approach
        public Vector2 CalculateApproachDirection(Perceivable perceivable, Vector3 inputDirection, float inputDistance, CircleMode mode, bool debug = false)
        {
            //Calculate index of current direction
            float inputAngle = Vector3.SignedAngle(Vector3.up, -inputDirection, -Vector3.forward);
            if (inputAngle < 0) inputAngle += 360.0f;
            int inputIndex = Mathf.RoundToInt(Mathf.Lerp(0, rayCount - 1, Mathf.InverseLerp(0, 360, inputAngle)));

            //If obstacle blocks path stay still
            if (obstacleSegments[inputIndex].Occupied(mode))
            {
                if (debug) Debug.DrawRay(transform.position, obstacleSegments[inputIndex].direction * nearDistance * 2.5f, Color.red);
                return Vector2.zero;
            }

            //Calculate extreme in each direction from obstacles
            float clockwise = GetClockwiseSpace(inputIndex, mode);
            float counterClockwise = GetCounterClockwiseSpace(inputIndex, mode);

            if (debug)
            {
                Debug.DrawRay(transform.position, -inputDirection * nearDistance * 2.5f, Color.red);
                Debug.DrawRay(transform.position, obstacleSegments[inputIndex].direction * nearDistance * 2.5f, Color.blue);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -clockwise) * obstacleSegments[inputIndex].direction * nearDistance * 2.5f, Color.cyan);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, counterClockwise) * obstacleSegments[inputIndex].direction * nearDistance * 2.5f, Color.green);
            }

            //Track crowding
            float crowding = CircularAntiCrowding(perceivable, inputDirection, inputDistance, ref clockwise, ref counterClockwise, debug);

            //Return circle direction
            if (clockwise > counterClockwise) return new Vector2(Mathf.Min(60, clockwise), crowding);
            else if (clockwise < counterClockwise) return new Vector2(-Mathf.Min(60, counterClockwise), crowding);
            else return new Vector2(0, crowding);
        }

        //Crowding
        public Vector2 SelfAntiCrowding(Perceivable target, Vector3 direction)
        {
            //Track crowding
            Vector3 nearestVector = Vector2.zero;
            float nearestDistance = Mathf.Infinity;

            for (int i = 0; i < perceivables.Count; i++)
            {
                //Grab other
                Perceivable other = perceivables[i];

                //Must be valid
                if (other == null) continue;

                //Don't test against self or target
                if (other == this) continue;
                if (other == target) continue;

                //Calculate vector to unit
                Vector3 otherVector = other.transform.position - transform.position;
                float otherDistance = otherVector.magnitude;

                //Distance check
                if (otherDistance < crowdingRadius && otherDistance < nearestDistance)
                {
                    //Set nearest vector
                    nearestVector = otherVector;

                    //Set nearest distance
                    nearestDistance = otherDistance;
                }
            }

            //Calculate direction to unit
            Vector3 nearestDirection = nearestVector.normalized;

            //Calculate relative direction
            Vector3 relativeDirection = transform.InverseTransformDirection(-nearestDirection);

            //Calculate severity
            float crowdingStrength = (1 - Mathf.Clamp01(nearestDistance / crowdingRadius)) * crowdingSpread;

            //Calculate anti-crowding
            Vector3 crowding = relativeDirection * crowdingStrength;

            Debug.DrawRay(transform.position, nearestDirection * 0.1f, Color.red);

            //Return total crowding
            return crowding;
        }
        private float CircularAntiCrowding(Perceivable perceivable, Vector3 inputDirection, float inputDistance, ref float clockwise, ref float counterClockwise, bool debug = false)
        {
            //Track crowding
            float crowding = 0;

            for (int i = 0; i < perceivables.Count; i++)
            {
                //Grab other
                Perceivable other = perceivables[i];

                //Must be valid
                if (other == null) continue;

                //Don't test against self or target
                if (other == this) continue;
                if (other == perceivable) continue;

                //Calculate vector to unit
                Vector3 otherVector = other.transform.position - transform.position;
                float otherDistance = otherVector.magnitude;

                //Distance check
                if (otherDistance > inputDistance * 1.2f) continue;

                //Calculate direction to unit
                Vector3 otherDirection = otherVector.normalized;

                //Calculate clockwise 
                float clockwiseOther = Vector3.SignedAngle(-inputDirection, otherDirection, -Vector3.forward);
                if (clockwiseOther < 0) clockwiseOther += 360.0f;

                //Calculate counter-clockwise
                float counterClockwiseOther = Vector3.SignedAngle(otherDirection , -inputDirection, -Vector3.forward);
                if (counterClockwiseOther < 0) counterClockwiseOther += 360.0f;

                //If nearby allies are closer to the target than us, move backwards & let them go first
                if ((clockwiseOther < 45 || counterClockwiseOther < 45) && other.Faction == faction && otherDistance < inputDistance) crowding -= 0.4f;

                //Update ref values if closer
                if (clockwiseOther < clockwise)
                {
                    clockwise = clockwiseOther;
                    if (debug) Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -clockwiseOther) * -inputDirection * nearDistance * 2.5f, Color.magenta);
                }
                if (counterClockwiseOther < counterClockwise)
                {
                    counterClockwise = counterClockwiseOther;
                    if (debug) Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, counterClockwiseOther) * -inputDirection * nearDistance * 2.5f, Color.yellow);
                }
            }

            //Return total crowding
            return crowding;
        }

        //Obstacles
        public void UpdateObstacles()
        {
            //Update each segment
            for (int i = 0; i < rayCount; i++)
            {
                obstacleSegments[i].Update(transform.position, nearDistance, farDistance, obstacleMask);
                if (debugObstacles) Debug.DrawRay(transform.position, obstacleSegments[i].direction * nearDistance, obstacleSegments[i].Occupied(CircleMode.Near)? Color.cyan: Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, rayCount, i)));
            }
        }

        //Obstacle Utility
        private float GetClockwiseSpace(int index, CircleMode mode)
        {
            float spacing = 0;
            float spacePerSegment = 360 / rayCount;
            for (int i = 0; i < rayCount; i++)
            {
                if (obstacleSegments[index].Occupied(mode)) return spacing;
                else
                {
                    index = GetClockwiseIndex(index);
                    spacing += spacePerSegment;
                }
            }
            return spacing;
        }
        private float GetCounterClockwiseSpace(int index, CircleMode mode)
        {
            float spacing = 0;
            float spacePerSegment = 360 / rayCount;
            for (int i = 0; i < rayCount; i++)
            {
                if (obstacleSegments[index].Occupied(mode)) return spacing;
                else
                {
                    index = GetCounterClockwiseIndex(index);
                    spacing += spacePerSegment;
                }
            }
            return spacing;
        }

        private int GetClockwiseIndex(int index)
        {
            int newIndex = index + 1;
            if (newIndex > rayCount - 1) newIndex = 0;
            return newIndex;
        }
        private int GetCounterClockwiseIndex(int index)
        {
            int newIndex = index - 1;
            if (newIndex < 0) newIndex = rayCount - 1;
            return newIndex;
        }

        //Death
        public void ConvertToCorpse()
        {
            if (perceivables == null) return;
            perceivables.Remove(this);

            //Add to corpses
            if (producesCorpse)
            {
                if (corpses == null) corpses = new List<Perceivable>();
                corpses.Add(this);
            }

            //Set as corpse layer
            gameObject.layer = corpseLayer;
        }
    }

    public class ObstacleSegment
    {
        public Vector3 direction;
        private bool occupiedNear;
        private bool occupiedFar;

        //Constructor
        public ObstacleSegment(int index, int count)
        {
            float lerp = Mathf.InverseLerp(0, count - 1, index);
            direction = Quaternion.Euler(0, 0, 360 - Mathf.Lerp(0, 360, lerp)) * Vector3.up;
        }

        //Access
        public bool Occupied(CircleMode mode)
        {
            if (mode == CircleMode.Near) return occupiedNear;
            else return occupiedFar;
        }
        public void Update(Vector3 position, float nearDistance, float farDistance, LayerMask mask)
        {
            occupiedNear = Physics2D.Raycast(position, direction, nearDistance, mask);
            occupiedFar = Physics2D.Raycast(position, direction, farDistance, mask);
        }
    }

    public enum Faction { Player, Hostile, Neutral }
    public enum CircleMode { Near, Far }
    public enum CircleDirection { Still, CounterClockwise, Clockwise }
}