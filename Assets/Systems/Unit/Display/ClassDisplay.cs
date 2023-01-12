using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwoBears.Persistance;

namespace TwoBears.Unit
{
    public class ClassDisplay : MonoBehaviour
    {
        [Header("Class")]
        public UnitClass unitClass;

        [Header("Source")]
        [SerializeField] private PlayerState state;

        [Header("Colours")]
        [SerializeField] private Color full = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        [SerializeField] private Color empty = new Color(0.4f, 0.4f, 0.4f, 1.0f);

        [Header("Flashing")]
        [SerializeField] private float flashDuration = 0.2f;

        [Header("Nubs")]
        [SerializeField] private Image nub1;
        [SerializeField] private Image nub2;
        [SerializeField] private Image nub3;
        [SerializeField] private Image nub4;
        [SerializeField] private Image nub5;
        [SerializeField] private Image nub6;

        [Header("Links")]
        [SerializeField] private Image link2;
        [SerializeField] private Image link4;
        [SerializeField] private Image link6;

        //Internal count
        private int count;
        private int trueCount;

        //Upgrade flash
        public int UnitID
        {
            set
            { 
                //Set ID
                unitId = value;

                //Update immediately
                UpdateCount();
            }
        }
        private int unitId = -1;

        private bool flashUpgrade;
        private float timeToToggle;

        //Mono
        private void Awake()
        {
            //Register to receive updates
            state.OnFormationChange += UpdateCount;
        }
        private void Update()
        {
            UpdateFlashing(Time.deltaTime);
        }
        private void OnDestroy()
        {
            state.OnFormationChange -= UpdateCount;
        }

        //Core
        private void UpdateCount()
        {
            //Get count
            count = state.GetCount(unitClass);

            //Get upgrade status
            if (unitId >= 0) flashUpgrade = !state.UnitInFormation(unitId);
            else flashUpgrade = false;

            //Update
            UpdateNubs(count);
        }
        private void UpdateNubs(int count)
        {
            //Set count
            trueCount = count;

            //Nubs
            nub1.color = (count >= 1) ? full : empty;
            nub2.color = (count >= 2) ? full : empty;

            if (nub3 != null) nub3.color = (count >= 3) ? full : empty;
            if (nub4 != null) nub4.color = (count >= 4) ? full : empty;
            if (nub5 != null) nub5.color = (count >= 5) ? full : empty;
            if (nub6 != null) nub6.color = (count >= 6) ? full : empty;

            //Links
            if (link2 != null) link2.color = (count >= 2) ? full : empty;
            if (link4 != null) link4.color = (count >= 4) ? full : empty;
            if (link6 != null) link6.color = (count >= 6) ? full : empty;
        }

        //Flashing
        private void UpdateFlashing(float deltaTime)
        {
            if (flashUpgrade)
            {
                timeToToggle -= deltaTime;
                if (timeToToggle <= 0)
                {
                    ToggleNubs();
                    timeToToggle = flashDuration;
                }
            }
        }
        private void ToggleNubs()
        {
            if (count == trueCount) UpdateNubs(count + 1);
            else UpdateNubs(count);
        }
    }
}