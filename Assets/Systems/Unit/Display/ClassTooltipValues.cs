using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TwoBears.Unit
{
    public class ClassTooltipValues : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private TextMeshProUGUI two;
        [SerializeField] private TextMeshProUGUI four;
        [SerializeField] private TextMeshProUGUI six;

        [Header("Colors")]
        [SerializeField] private Color active = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        [SerializeField] private Color inactive = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        //Display
        public void UpdateDisplay(int count)
        {
            if (two != null) two.color = (count >= 2) ? active : inactive;
            if (four != null) four.color = (count >= 4) ? active : inactive;
            if (six != null) six.color = (count >= 6) ? active : inactive;
        }
    }
}