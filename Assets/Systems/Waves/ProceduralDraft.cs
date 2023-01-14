using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Waves
{
    [CreateAssetMenu(menuName = "TwoBears/Procedural Draft")]
    public class ProceduralDraft : ScriptableObject
    {
        [Header("Parent")]
        [SerializeField] private ProceduralDraft[] parents;

        [Header("Units")]
        [SerializeField] private ProceduralUnit[] units;

        public ProceduralUnit[] Units
        {
            get
            {
                //Create a new list
                List<ProceduralUnit> collection = new List<ProceduralUnit>();

                //Recursively collect units
                CollectUnits(ref collection);

                //Return as array
                return collection.ToArray();
            }
        }
        public void CollectUnits(ref List<ProceduralUnit> collection)
        {
            //Collect units recursively
            if (parents != null)
            {
                //Collect from each parent
                for (int i = 0; i < parents.Length; i++)
                {
                    parents[i].CollectUnits(ref collection);
                }
            }

            //Add our units
            for (int i = 0; i < units.Length; i++) AddUnit(ref collection, units[i]);
        }
        private void AddUnit(ref List<ProceduralUnit> collection, ProceduralUnit unit)
        {
            //Check if unit is already present
            if (collection.Count > 0)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].id == unit.id && collection[i].level == unit.level)
                    {
                        //Update priority
                        collection[i].priority = Mathf.Max(collection[i].priority, unit.priority);

                        //Update placement
                        collection[i].front = collection[i].front || unit.front;
                        collection[i].middle = collection[i].middle || unit.middle;
                        collection[i].back = collection[i].back || unit.back;

                        //Unit updated successfully
                        return;
                    }
                }
            }

            //Unit not found -- Added
            collection.Add(unit);
        }
    }

    [System.Serializable]
    public class ProceduralUnit
    {
        [Header("Unit")]
        [Range(0, 99)] public int id = 0;
        [Range(0, 2)] public int level = 0;

        [Header("Allocation")]
        [Range(3, 30)] public int cost = 3;
        [Range(1, 10)] public int priority = 1;

        [Header("Placement")]
        public bool front;
        public bool middle;
        public bool back;

        //Unit conversion
        public FormationUnit Unit
        {
            get { return new FormationUnit(id, level); }
        }
    }
}