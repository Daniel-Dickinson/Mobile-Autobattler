using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBears.Loop
{
    [RequireComponent(typeof(AudioSource))]
    public class DemoUI : MonoBehaviour
    {
        //Components
        private AudioSource aud;

        private void Awake()
        {
            aud = GetComponent<AudioSource>();
        }
        public void OnComplete()
        {
            //Play defeat audio
            aud.Play();
        }
    }
}