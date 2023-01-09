using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Loop
{
    [RequireComponent(typeof(AudioSource))]
    public class CompleteUI : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;

        [Header("AdMenu")]
        public GameObject adMenu;

        [Header("Reward")]
        public int shardReward = 1;
        public int shardAdReward = 5;

        //Components
        private AudioSource aud;

        private void Awake()
        {
            aud = GetComponent<AudioSource>();
        }
        private void OnEnable()
        {
            //Play victory audio
            aud.Play();

        //#if UNITY_IOS || UNITY_ANDRIOD
            if ((state.Wave - 1) % 5 != 0 || adMenu == null) state.Shards += shardReward;
            else adMenu.SetActive(true);
        //#else
            //state.Shards += shardReward;
        //#endif
        }
    }
}