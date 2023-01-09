using System.Collections.Generic;
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

        [Header("Obstacles")]
        public int rayCount = 32;
        public float rayDistance = 2.0f;
        public LayerMask obstacleMask;

        [Header("Debug")]
        [SerializeField] protected bool debugDirection = false; 
        [SerializeField] protected bool debugObstacles = false;

        //Obstacles
        private ObstacleSegment[] obstacleSegments;

        //Data
        protected static List<Perceivable> perceivables;

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
        private void Update()
        {
            UpdateObstacles();
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
        }

        //Approach
        public Vector2 CalculateApproachDirection(Perceivable perceivable, Vector3 inputDirection, bool debug = false)
        {
            //Calculate index of current direction
            float inputAngle = Vector3.SignedAngle(Vector3.up, -inputDirection, -Vector3.forward);
            if (inputAngle < 0) inputAngle += 360.0f;
            int inputIndex = Mathf.RoundToInt(Mathf.Lerp(0, rayCount - 1, Mathf.InverseLerp(0, 360, inputAngle)));

            //If obstacle blocks path stay still
            if (obstacleSegments[inputIndex].Occupied)
            {
                if (debug) Debug.DrawRay(transform.position, obstacleSegments[inputIndex].direction * rayDistance * 2.5f, Color.red);
                return Vector2.zero;
            }

            //Calculate extreme in each direction from obstacles
            float clockwise = GetClockwiseSpace(inputIndex);
            float counterClockwise = GetCounterClockwiseSpace(inputIndex);

            if (debug)
            {
                Debug.DrawRay(transform.position, -inputDirection * rayDistance * 2.5f, Color.red);
                Debug.DrawRay(transform.position, obstacleSegments[inputIndex].direction * rayDistance * 2.5f, Color.blue);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -clockwise) * obstacleSegments[inputIndex].direction * rayDistance * 2.5f, Color.cyan);
                Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, counterClockwise) * obstacleSegments[inputIndex].direction * rayDistance * 2.5f, Color.green);
                //Debug.Log("CW (Cyan): " + clockwise + " -- " + "CCW (Green): " + counterClockwise);
            }

            //Track crowding
            float crowding = AntiCrowding(perceivable, inputDirection, ref clockwise, ref counterClockwise, debug);

            //Return circle direction
            if (clockwise > counterClockwise) return new Vector2(Mathf.Min(60, clockwise), crowding);
            else if (clockwise < counterClockwise) return new Vector2(-Mathf.Min(60, counterClockwise), crowding);
            else return Vector2.zero;
        }

        //Crowding
        private float AntiCrowding(Perceivable perceivable, Vector3 inputDirection, ref float clockwise, ref float counterClockwise, bool debug = false)
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
                if (otherDistance > rayDistance * 2.0f) continue;

                //Calculate direction to unit
                Vector3 otherDirection = otherVector.normalized;

                //Calculate clockwise 
                float clockwiseOther = Vector3.SignedAngle(-inputDirection, otherDirection, -Vector3.forward);
                if (clockwiseOther < 0) clockwiseOther += 360.0f;

                //Calculate counter-clockwise
                float counterClockwiseOther = Vector3.SignedAngle(otherDirection , -inputDirection, -Vector3.forward);
                if (counterClockwiseOther < 0) counterClockwiseOther += 360.0f;

                //Update ref values if closer
                if (clockwiseOther < clockwise)
                {
                    clockwise = clockwiseOther;
                    if (debug) Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -clockwiseOther) * -inputDirection * rayDistance * 2.5f, Color.magenta);
                }
                if (counterClockwiseOther < counterClockwise)
                {
                    counterClockwise = counterClockwiseOther;
                    if (debug) Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, counterClockwiseOther) * -inputDirection * rayDistance * 2.5f, Color.yellow);
                }
            }

            //Return total crowding
            return crowding;
        }

        //Obstacles
        private void UpdateObstacles()
        {
            //Update each segment
            for (int i = 0; i < rayCount; i++)
            {
                obstacleSegments[i].Update(transform.position, rayDistance, obstacleMask);
                if (debugObstacles) Debug.DrawRay(transform.position, obstacleSegments[i].direction * rayDistance, obstacleSegments[i].Occupied? Color.cyan: Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0, rayCount, i)));
            }
        }

        //Obstacle Utility
        private float GetClockwiseSpace(int index)
        {
            float spacing = 0;
            float spacePerSegment = 360 / rayCount;
            for (int i = 0; i < rayCount; i++)
            {
                if (obstacleSegments[index].Occupied) return spacing;
                else
                {
                    index = GetClockwiseIndex(index);
                    spacing += spacePerSegment;
                }
            }
            return spacing;
        }
        private float GetCounterClockwiseSpace(int index)
        {
            float spacing = 0;
            float spacePerSegment = 360 / rayCount;
            for (int i = 0; i < rayCount; i++)
            {
                if (obstacleSegments[index].Occupied) return spacing;
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
    }

    public class ObstacleSegment
    {
        public Vector3 direction;
        private bool occupied;

        //Constructor
        public ObstacleSegment(int index, int count)
        {
            float lerp = Mathf.InverseLerp(0, count - 1, index);
            direction = Quaternion.Euler(0, 0, 360 - Mathf.Lerp(0, 360, lerp)) * Vector3.up;
        }

        //Access
        public bool Occupied
        {
            get { return occupied; }
        }
        public void Update(Vector3 position, float distance, LayerMask mask)
        {
            occupied = Physics2D.Raycast(position, direction, distance, mask);
        }
    }

    public enum Faction { Player, Hostile, Neutral }
    public enum CircleDirection { Still, CounterClockwise, Clockwise }
}