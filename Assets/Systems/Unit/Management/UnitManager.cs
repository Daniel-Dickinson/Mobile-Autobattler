using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class UnitManager : MonoBehaviour
    {
        [Header("Timings")]
        [SerializeField] private int obstacleOpsPerSecond = 5;
        [SerializeField] private int targetingOpsPerSecond = 5;
        [SerializeField] private int pathfindingOpsPerSecond = 5;
        [SerializeField] private int anticrowdingOpsPerSecond = 10;

        //Tracking
        private static int obstacleIndex;
        private static int targetingIndex;
        private static int pathfindingIndex;
        private static int crowdingIndex;

        //Dependency Injection
        private static List<BaseUnit> units;

        public static void RegisterUnit(BaseUnit unit)
        {
            if (units == null) units = new List<BaseUnit>();

            //Add unit
            units.Add(unit);

            //Kickstart unit
            unit.UpdateObstacles();
            unit.UpdateTargeting();
            unit.UpdatePathfinding();
            unit.UpdateAntiCrowding();
        }
        public static void DeregisterUnit(BaseUnit unit)
        {
            if (units == null) return;

            //Remove unit
            units.Remove(unit);

            //Check all indicies
            if (obstacleIndex >= units.Count) obstacleIndex = 0;
            if (targetingIndex >= units.Count) targetingIndex = 0;
            if (pathfindingIndex >= units.Count) pathfindingIndex = 0;
            if (crowdingIndex >= units.Count) crowdingIndex = 0;
        }

        //Mono
        private void FixedUpdate()
        {
            UpdateUnits(Time.deltaTime);
        }

        //Operation
        private void UpdateUnits(float deltaTime)
        {
            //Units required
            if (units == null || units.Count == 0) return;

            //Perform operations
            PerformObstacleOperations(deltaTime);
            PerformTargetingOperations(deltaTime);
            PerformPathfindingOperations(deltaTime);
            PerformCrowdingOperations(deltaTime);
        }

        private void PerformObstacleOperations(float deltaTime)
        {
            //Calculate operations required this frame to meet timings
            int operationsPerSecond = units.Count * obstacleOpsPerSecond;   //Must be dynamic as unit count can change rapidly
            int operationsPerFrame = Mathf.CeilToInt(operationsPerSecond * deltaTime);

            //Update allocated units for this frame (obstacles always required)
            for (int i = 0; i < operationsPerFrame; i++)
            {
                units[obstacleIndex].UpdateObstacles();

                //Update obstacle index
                obstacleIndex++;
                if (obstacleIndex >= units.Count) obstacleIndex = 0;
            }
        }
        private void PerformTargetingOperations(float deltaTime)
        {
            //Calculate operations required this frame to meet timings
            int operationsPerSecond = units.Count * targetingOpsPerSecond;   //Must be dynamic as unit count can change rapidly
            int operationsPerFrame = Mathf.CeilToInt(operationsPerSecond * deltaTime);

            //Update allocated units for this frame if required
            for (int i = 0; i < operationsPerFrame; i++)
            {
                if (units[targetingIndex].TargetingRequired) units[targetingIndex].UpdateTargeting();

                //Update targeting index
                targetingIndex++;
                if (targetingIndex >= units.Count) targetingIndex = 0;
            }
        }
        private void PerformPathfindingOperations(float deltaTime)
        {
            //Calculate operations required this frame to meet timings
            int operationsPerSecond = units.Count * pathfindingOpsPerSecond;   //Must be dynamic as unit count can change rapidly
            int operationsPerFrame = Mathf.CeilToInt(operationsPerSecond * deltaTime);

            //Update allocated units for this frame if required
            for (int i = 0; i < operationsPerFrame; i++)
            {
                if (units[pathfindingIndex].PathfindingRequired) units[pathfindingIndex].UpdatePathfinding();

                //Update pathfinding index
                pathfindingIndex++;
                if (pathfindingIndex >= units.Count) pathfindingIndex = 0;
            }
        }
        private void PerformCrowdingOperations(float deltaTime)
        {
            //Calculate operations required this frame to meet timings
            int operationsPerSecond = units.Count * anticrowdingOpsPerSecond;   //Must be dynamic as unit count can change rapidly
            int operationsPerFrame = Mathf.CeilToInt(operationsPerSecond * deltaTime);

            //Update allocated units for this frame if required
            for (int i = 0; i < operationsPerFrame; i++)
            {
                if (!units[crowdingIndex].AntiCrowdingRequired) continue;
                units[crowdingIndex].UpdateAntiCrowding();

                //Update crowding index
                crowdingIndex++;
                if (crowdingIndex >= units.Count) crowdingIndex = 0;
            }
        }
    }
}