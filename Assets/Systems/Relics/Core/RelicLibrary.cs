using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Relics
{
    [CreateAssetMenu(menuName ="TwoBears/Relic Library")]
    public class RelicLibrary : ScriptableObject
    {
        public RelicUnlock[] unlockables;

        public Relic GetRandomUnlockedRelic(List<int> existing, List<int> exclude)
        {
            int safety = 0; 

            //Pick a random relic
            RelicUnlock unlock = unlockables[Random.Range(0, unlockables.Length)];

            //Check if unlocked or in exluded relics
            while ((!unlock.unlocked || existing.Contains(unlock.relic.ID) || exclude.Contains(unlock.relic.ID)) && safety < 100)
            {
                //Try another relic until we find one thats unlocked
                unlock = unlockables[Random.Range(0, unlockables.Length)];

                //Prevent infinite loop
                safety++;

                //Should be faster than building a list of all unlocked relics then picking one
                //Will refactor to maintain a seperate list on load at some point
            }

            return unlock.relic;
        }
        public Relic GetRelicFromID(int id)
        {
            for (int i = 0; i < unlockables.Length; i++)
            {
                if (unlockables[i].relic.ID == id) return unlockables[i].relic;
            }

            //Relic not found
            return null;
        }
    }

    [System.Serializable]
    public class RelicUnlock
    {
        public Relic relic;
        public bool unlocked = true;
        public int level = 0;
    }
}