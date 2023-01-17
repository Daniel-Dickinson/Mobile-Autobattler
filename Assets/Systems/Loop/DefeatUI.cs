using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Loop
{
    [RequireComponent(typeof(AudioSource))]
    public class DefeatUI : MonoBehaviour
    {
        //Components
        private AudioSource aud;

        private void Awake()
        {
            aud = GetComponent<AudioSource>();
        }

        public void OnDefeat()
        {
            //Play defeat audio
            aud.Play();
        }
    }
}