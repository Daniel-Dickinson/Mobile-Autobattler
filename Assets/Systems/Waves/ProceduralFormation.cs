using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TwoBears.Waves
{
    [CreateAssetMenu(menuName = "TwoBears/Procedural Formation")]
    public class ProceduralFormation : Formation
    {
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
            UpdateUnlockedUnits(proceduralWave.draft, wave);

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
        private void UpdateUnlockedUnits(ProceduralDraft draft, int wave)
        {
            //Initialize or clear unit lists
            if (frontUnlocked == null) frontUnlocked = new List<ProceduralUnit>();
            else frontUnlocked.Clear();

            if (middleUnlocked == null) middleUnlocked = new List<ProceduralUnit>();
            else middleUnlocked.Clear();

            if (backUnlocked == null) backUnlocked = new List<ProceduralUnit>();
            else backUnlocked.Clear();

            //Grab units
            ProceduralUnit[] units = draft.Units;

            //Populate lists
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].front) frontUnlocked.Add(units[i]);
                if (units[i].middle) middleUnlocked.Add(units[i]);
                if (units[i].back) backUnlocked.Add(units[i]);
            }

            //Sort by priority then cost
            if (frontUnlocked.Count > 1) frontUnlocked.Sort(delegate (ProceduralUnit a, ProceduralUnit b) 
            {
                if (b.priority == a.priority) return b.cost.CompareTo(a.cost);
                else return b.priority.CompareTo(a.priority);
            });
            if (middleUnlocked.Count > 1) middleUnlocked.Sort(delegate (ProceduralUnit a, ProceduralUnit b)
            {
                if (b.priority == a.priority) return b.cost.CompareTo(a.cost);
                else return b.priority.CompareTo(a.priority);
            });
            if (backUnlocked.Count > 1) backUnlocked.Sort(delegate (ProceduralUnit a, ProceduralUnit b)
            {
                if (b.priority == a.priority) return b.cost.CompareTo(a.cost);
                else return b.priority.CompareTo(a.priority);
            });
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
        [Header("Content")]
        public ProceduralDraft draft;
        public int reward = 3;

        [Header("Points")]
        [Range(0, 1500)] public int frontPoints = 3;
        [Range(0, 1500)] public int middlePoints = 6;
        [Range(0, 1500)] public int backPoints = 0;

        public int Total
        {
            get { return frontPoints + middlePoints + backPoints; }
        }
    }
}