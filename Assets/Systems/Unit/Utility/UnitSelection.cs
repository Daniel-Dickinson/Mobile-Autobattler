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
        public UnitType warrior;
        public UnitType spellBlade;

        [Header("Defenders")]
        public UnitType squire;
        public UnitType knight;
        public UnitType paladin;

        [Header("Rangers")]
        public UnitType slinger;
        public UnitType archer;
        public UnitType assassin;

        [Header("Healers")]
        public UnitType priest;

        [Header("Casters")]
        public UnitType wizard;

        [Header("Summoners")]
        public UnitType swarm;
        public UnitType necromancer;
        public UnitType broodmother;

        //Request
        public GameObject GetUnit(int id, int level)
        {
            switch (id)
            {
                //Empty
                default:
                    return null;

                //Warriors
                case 0:
                    return serf.GetUnit(level);
                case 1:
                    return warrior.GetUnit(level);
                case 2:
                    return spellBlade.GetUnit(level);

                //Defenders
                case 10:
                    return squire.GetUnit(level);
                case 11:
                    return knight.GetUnit(level);
                case 12:
                    return paladin.GetUnit(level);

                //Rangers
                case 20:
                    return slinger.GetUnit(level);
                case 21:
                    return archer.GetUnit(level);
                case 23:
                    return assassin.GetUnit(level);

                //Healers
                case 30:
                    return priest.GetUnit(level);

                //Casters
                case 40:
                    return wizard.GetUnit(level);

                //Summoners

                case 60:
                    return swarm.GetUnit(level);
                case 61:
                    return necromancer.GetUnit(level);
                case 62:
                    return broodmother.GetUnit(level);
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