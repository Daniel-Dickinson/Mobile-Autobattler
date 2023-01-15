using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Relics
{
    public class RelicDisplay : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private PlayerState state;

        [Header("Components")]
        [SerializeField] private CanvasGroup botBar;

        [Header("Metrics")]
        [SerializeField] private int baseSize = 40;
        [SerializeField] private int iconWidth = 40;
        [SerializeField] private int iconSpacing = 20;

        RectTransform botBarRect;

        //Mono
        private void Awake()
        {
            state.OnRelicsChange += UpdateDisplay;

            //Grab rect transform
            botBarRect = botBar.transform as RectTransform;

            //Update
            UpdateDisplay();
        }
        private void OnDestroy()
        {
            state.OnRelicsChange -= UpdateDisplay;
        }

        //Core
        private void UpdateDisplay()
        {
            //Grab our relics
            List<Relic> relics = state.GetRelicObjects;

            //Destroy existing children
            for (int i = transform.childCount - 1; i >= 0; i--) Destroy(transform.GetChild(i).gameObject);
            
            if (relics.Count > 0)
            {
                //Show bot bar
                botBar.alpha = 1;

                //Resize bot bar
                botBarRect.sizeDelta = new Vector2(baseSize + (relics.Count * iconWidth) + ((relics.Count - 1) * iconSpacing), botBarRect.sizeDelta.y);

                //Add a new copy of each relic -- Clean & reliable
                for (int i = 0; i < relics.Count; i++)
                {
                    Instantiate(relics[i], transform, false);
                }
            }
            else
            {
                //Hide bot bar
                botBar.alpha = 0;
            }
            
        }
    }
}