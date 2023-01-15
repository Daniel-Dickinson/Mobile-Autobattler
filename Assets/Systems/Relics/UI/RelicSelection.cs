using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;

namespace TwoBears.Relics
{
    public class RelicSelection : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerState state;

        [Header("Info")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI buttonTitle;
        [SerializeField] private TextMeshProUGUI info;

        [Header("Selection")]
        [SerializeField] private RelicSlot[] slots;

        //Return Event
        public Action OnItemChosen;

        //Exclud
        List<int> exclude;

        //Access
        public void Initialize()
        {
            if (exclude == null) exclude = new List<int>();
            else exclude.Clear();

            //Initalize each slot
            for (int i = 0; i < slots.Length; i++)
            {
                //Grab new relic
                Relic source = state.GetRandomUnlockedRelic(exclude);

                //Exclude relic to remove doubles
                exclude.Add(source.ID);

                //Spawn icon for relic
                slots[i].Initialize(this, i, source);
            }

            //Select center slot
            slots[1].Select();
        }
        public void SelectSocket(Relic relic)
        {
            //Update info
            title.text = relic.title;
            buttonTitle.text = "Take " + relic.title;
            info.text = relic.information;
        }
        public void ConfirmSelection()
        {
            int relicID = slots[RelicSlot.selected].RelicID;

            //Add item to relics
            state.AddRelic(relicID);

            //Item selected
            OnItemChosen?.Invoke();
        }
    }
}