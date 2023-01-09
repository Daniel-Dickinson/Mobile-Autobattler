using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Waves
{
    [CreateAssetMenu(menuName = "TwoBears/Wave Selection")]
    public class WaveSelection : ScriptableObject
    {
        public Formation[] waves;
    }
}