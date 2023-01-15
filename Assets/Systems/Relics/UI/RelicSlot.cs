using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;
using System;

namespace TwoBears.Relics
{
    public class RelicSlot : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CanvasGroup hover;
        [SerializeField] private RectTransform socket;

        private int index;
        private Relic relic;
        private int relicID;

        //Parent
        private RelicSelection selection;

        //Selection
        public static int selected = 0;
        private static Action OnSelection;

        //Mono
        private void Awake()
        {
            OnSelection += UpdateDisplay;
        }
        private void OnDestroy()
        {
            OnSelection -= UpdateDisplay;
        }

        //Access
        public Relic Relic
        {
            get { return relic; }
        }
        public int RelicID
        {
            get { return relicID; }
        }

        //Core
        public void Initialize(RelicSelection selection, int index, Relic source)
        {
            this.index = index;
            this.selection = selection;

            //Clear old icons
            if (socket.childCount > 0)
            {
                for (int c = socket.childCount - 1; c >= 0; c--) Destroy(socket.GetChild(c).gameObject);
            }

            //Spawn icon for new relic
            relic = Instantiate(source, socket, false);
            relicID = source.ID;

            //Attach to event
            relic.OnPress += Select;

            //Update display
            UpdateDisplay();
        }

        //Select
        public void Select()
        {
            selected = index;
            OnSelection?.Invoke();

            //Tell selection
            selection.SelectSocket(relic);
        }

        //Display
        private void UpdateDisplay()
        {
            //Set alpha
            hover.alpha = (selected == index) ? 1.0f : 0.0f;
        }
    }
}