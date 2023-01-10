using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoBears.Unit
{
    public class ClassDisplay : MonoBehaviour
    {
        [Header("Class")]
        [SerializeField] private UnitClass unitClass;

        [Header("Source")]
        [SerializeField] private ClassCounter counter;

        [Header("Colours")]
        [SerializeField] private Color full = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        [SerializeField] private Color empty = new Color(0.4f, 0.4f, 0.4f, 1.0f);

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

        //Mono
        private void Awake()
        {
            counter.OnCount += UpdateCount;
        }
        private void OnDestroy()
        {
            counter.OnCount -= UpdateCount;
        }

        //Core
        private void UpdateCount()
        {
            UpdateNubs(counter.GetCount(unitClass));
        }
        private void UpdateNubs(int count)
        {
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
    }
}