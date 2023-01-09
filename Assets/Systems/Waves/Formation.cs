using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Unit;

namespace TwoBears.Waves
{
    [CreateAssetMenu(menuName = "TwoBears/Formation")]
    public class Formation : ScriptableObject
    {
        public FormationUnit[] front;
        public FormationUnit[] middle;
        public FormationUnit[] back;
    }

    [Serializable]
    public class FormationUnit
    {
        public int id;
        public int level;
        public int sublevel;

        public Action OnStack;

        //Constructors
        public FormationUnit()
        {
            id = -1;
            level = 0;
            sublevel = 0;
        }
        public FormationUnit(int id)
        {
            this.id = id;

            level = 0;
            sublevel = 0;
        }
        public FormationUnit(int id, int level)
        {
            this.id = id;
            this.level = level;
            sublevel = 0;
        }
        public FormationUnit(int id, int level, int sublevel)
        {
            this.id = id;
            this.level = level;
            this.sublevel = sublevel;
        }

        //Stacking
        public bool Stackable(int stacks)
        {
            return (level < 2) && (sublevel + stacks) < 3;
        }
        public void Stack(int stacks)
        {
            //Increment sublevel
            sublevel += stacks;

            //Increment level
            if (sublevel >= 2)
            {
                sublevel = 0;
                level++;
            }

            //Stacked
            OnStack?.Invoke();
        }
    }
}