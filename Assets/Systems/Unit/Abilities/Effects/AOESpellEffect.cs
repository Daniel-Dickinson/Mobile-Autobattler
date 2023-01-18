using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Unit
{
    public class AOESpellEffect : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform indicator;

        //Access
        public void SetIndicator(float value)
        {
            indicator.localScale = new Vector3(value, value, value);
        }
    }
}