using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TwoBears.Waves
{
    [CreateAssetMenu(menuName = "TwoBears/Procedural Formation")]
    public class ProceduralFormation : Formation
    {
        [SerializeField] private ProceduralUnit[] units;
        [SerializeField] private ProceduralWave[] waves;

        //Unlocked units
        private List<ProceduralUnit> frontUnlocked;
        private List<ProceduralUnit> middleUnlocked;
        private List<ProceduralUnit> backUnlocked;

        //Rows
        private List<ProceduralUnit> frontUnits;
        private List<ProceduralUnit> middleUnits;
        private List<ProceduralUnit> backUnits;

#if UNITY_EDITOR
        //Editor Foldouts -- Serialized with object
        [SerializeField] private bool foldoutOne;
        [SerializeField] private bool foldoutTwo;
        [SerializeField] private bool foldoutThree;
        [SerializeField] private bool foldoutFour;
        [SerializeField] private bool foldoutFive;
        [SerializeField] private bool foldoutSix;
        [SerializeField] private bool foldoutSeven;
        [SerializeField] private bool foldoutEight;
        [SerializeField] private bool foldoutNine;
        [SerializeField] private bool foldoutTen;
#endif

        //Populate
        public void Populate(int wave)
        {
            //Grab procedural wave
            ProceduralWave proceduralWave = waves[wave];

            //Grab unlocked units
            UpdateUnlockedUnits(wave);

            //Populate rows
            PopulateRow(frontUnlocked, proceduralWave.frontPoints, ref frontUnits);
            PopulateRow(middleUnlocked, proceduralWave.middlePoints, ref middleUnits);
            PopulateRow(backUnlocked, proceduralWave.backPoints, ref backUnits);

            //Commit units to formation
            CommitToFormation();
        }

        //Rewards
        public int GetGoldReward(int wave)
        {
            return waves[wave].reward;
        }

        //Utility
        private void UpdateUnlockedUnits(int wave)
        {
            //Initialize or clear unit lists
            if (frontUnlocked == null) frontUnlocked = new List<ProceduralUnit>();
            else frontUnlocked.Clear();

            if (middleUnlocked == null) middleUnlocked = new List<ProceduralUnit>();
            else middleUnlocked.Clear();

            if (backUnlocked == null) backUnlocked = new List<ProceduralUnit>();
            else backUnlocked.Clear();

            //Populate lists
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].frontUnlocked >= 0 && units[i].frontUnlocked <= (wave + 1)) frontUnlocked.Add(units[i]);
                if (units[i].middleUnlocked >= 0 && units[i].middleUnlocked <= (wave + 1)) middleUnlocked.Add(units[i]);
                if (units[i].backUnlocked >= 0 && units[i].backUnlocked <= (wave + 1)) backUnlocked.Add(units[i]);
            }

            //Sort by cost
            frontUnlocked.Sort(delegate (ProceduralUnit a, ProceduralUnit b) { return b.cost.CompareTo(a.cost); });
            middleUnlocked.Sort(delegate (ProceduralUnit a, ProceduralUnit b) { return b.cost.CompareTo(a.cost); });
            backUnlocked.Sort(delegate (ProceduralUnit a, ProceduralUnit b) { return b.cost.CompareTo(a.cost); });
        }
        private void PopulateRow(List<ProceduralUnit> source, int points, ref List<ProceduralUnit> output)
        {
            //Initialize
            if (output == null) output = new List<ProceduralUnit>();
            else output.Clear();

            //Source units required
            if (source == null || source.Count == 0) return;

            //Spend points until we have 15 units or run out
            bool alternate = false;
            int cheapestUnit = source[source.Count - 1].cost;
            while (points >= cheapestUnit && output.Count < 15)
            {
                //Grab unit
                ProceduralUnit unit = GetMostExpensiveUnit(source, points);

                //Always add to the each side -- Keeps strongest units in the center
                alternate = !alternate;

                //Add unit
                if (alternate) output.Add(unit);
                else output.Insert(0, unit);

                //Subtract cost
                points -= unit.cost;
            }
        }
        private ProceduralUnit GetMostExpensiveUnit(List<ProceduralUnit> source, int points)
        {
            //Add highest cost unit possible
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].cost <= points) return source[i];
            }

            //No unit found under cost
            return null;
        }
        private void CommitToFormation()
        {
            front = new FormationUnit[frontUnits.Count];
            for (int f = 0; f < frontUnits.Count; f++) front[f] = frontUnits[f].Unit;

            middle = new FormationUnit[middleUnits.Count];
            for (int m = 0; m < middleUnits.Count; m++) middle[m] = middleUnits[m].Unit;

            back = new FormationUnit[backUnits.Count];
            for (int b = 0; b < backUnits.Count; b++) back[b] = backUnits[b].Unit;
        }

        //Points
        public int GetPlayerPointsForWave(int wave)
        {
            return GetPointValueForWave(wave) + 9;
        }
        private int GetPointValueForWave(int wave)
        {
            if (wave == 0) return 0;
            if (wave == 1) return waves[0].reward;
            else
            {
                //Get point gain from previous
                int total = GetPointValueForWave(wave - 1);

                //Add previous wave
                total += waves[wave - 1].reward;

                //Return total
                return total;
            }
        }
    }

    [System.Serializable]
    public class ProceduralWave
    {
        [Range(0, 1500)] public int frontPoints = 3;
        [Range(0, 1500)] public int middlePoints = 6;
        [Range(0, 1500)] public int backPoints = 0;
        public int reward = 3;

        public int Total
        {
            get { return frontPoints + middlePoints + backPoints; }
        }
    }

    [System.Serializable]
    public class ProceduralUnit
    {
        [Header("Unit")]
        [Range(0, 50)] public int id = 0;
        [Range(0, 2)] public int level = 0;

        [Header("Allocation")]
        [Range(3, 30)] public int cost = 3;

        [Header("Placement")]
        public int frontUnlocked = 0;
        public int middleUnlocked = -1;
        public int backUnlocked = -1;

        //Unit conversion
        public FormationUnit Unit
        {
            get { return new FormationUnit(id, level); }
        }
    }
}