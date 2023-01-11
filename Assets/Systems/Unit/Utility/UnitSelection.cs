using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    [CreateAssetMenu(menuName = "TwoBears/Unit Selection")]
    public class UnitSelection : ScriptableObject
    {
        [Header("Warriors")] 
        public UnitType serf;
        public UnitType swarm;

        [Header("Defenders")]
        public UnitType squire;
        public UnitType knight;

        [Header("Rangers")]
        public UnitType slinger;
        public UnitType archer;

        [Header("Healers")]
        public UnitType priest;

        //Request
        public GameObject GetUnit(int id, int level)
        {
            switch (id)
            {
                //Empty
                default:
                    return null;

                //Base
                case 0:
                    return serf.GetUnit(level);

                //Warriors
                case 1:
                    return swarm.GetUnit(level);

                //Defenders
                case 10:
                    return squire.GetUnit(level);
                case 11:
                    return knight.GetUnit(level);

                //Rangers
                case 20:
                    return slinger.GetUnit(level);
                case 21:
                    return archer.GetUnit(level);

                //Healers
                case 30:
                    return priest.GetUnit(level);
            }
        }
    }

    [System.Serializable]
    public class UnitType
    {
        public GameObject level01;
        public GameObject level02;
        public GameObject level03;

        public GameObject GetUnit(int level)
        {
            switch (level) 
            {
                default:
                case 0: return level01;
                case 1: return level02;
                case 2: return level03;
            }
        }
    }
}