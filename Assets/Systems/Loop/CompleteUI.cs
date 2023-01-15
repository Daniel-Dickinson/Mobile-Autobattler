using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using TwoBears.Persistance;
using TwoBears.Waves;

namespace TwoBears.Loop
{
    [RequireComponent(typeof(AudioSource))]
    public class CompleteUI : MonoBehaviour
    {
        [Header("Data")]
        public PlayerState state;
        public ProceduralFormation formation;

        [Header("Titles")]
        public TextMeshProUGUI headline;
        public CanvasGroup lifeLost;

        [Header("Gold Reward")]
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI goldIncome;

        [Header("Merchant Reward")]
        public GameObject merchReward;
        public TextMeshProUGUI merchIncome;

        [Header("Shard Reward")]
        public GameObject shardReward;
        public TextMeshProUGUI shardText;
        public TextMeshProUGUI shardIncome;

        [Header("Audio")]
        public AudioClip successAudio;
        public AudioClip defeatAudio;

        [Header("AdMenu")]
        public GameObject adMenu;

        //Components
        private AudioSource aud;

        private void Awake()
        {
            aud = GetComponent<AudioSource>();
        }

        public void OnComplete()
        {
            //Play victory audio
            aud.clip = successAudio;
            aud.Play();

            //Set headline text
            headline.text = "Wave Completed!";

            //Payout rewards
            PayoutRewards();
        }
        public void OnDefeat()
        {
            //Play defeat audio
            aud.clip = defeatAudio;
            aud.Play();

            //Set headline text
            headline.text = "Wave Failed";

            //Payout gold for previous wave
            PayoutRewards(false);
        }

        private void PayoutRewards(bool success = true)
        {
            //Calculate rewards
            int wave = state.Wave;
            int goldReward = formation.GetGoldReward(wave);
            int merchReward = (success) ? 0 : 0;
            int shardReward = (success) ? (((wave + 1) % 5 == 0) ? 1 : 0) : 0;

            //Update displays
            UpdateGoldDisplay(goldReward, wave, success);
            UpdateMerchantDisplay(merchReward);
            UpdateShardDisplay(shardReward, wave);

            //Update life lost
            lifeLost.gameObject.SetActive(!success);

            //#if UNITY_IOS || UNITY_ANDRIOD
            //if ((state.Wave - 1) % 5 != 0 || adMenu == null) state.Shards += 1;
            //else adMenu.SetActive(true);
            //#else
            //state.Shards += shardReward;
            //#endif

            //Payout
            state.Gold += goldReward + merchReward;
            state.Shards += shardReward;
        }
        private void UpdateGoldDisplay(int gold, int wave, bool success)
        {
            goldText.text = "Wave " + (wave + 1) + (success? " Completed" : " Failed");
            goldIncome.text = "+" + gold;
        }
        private void UpdateMerchantDisplay(int gold)
        {
            merchReward.SetActive(gold != 0);
            merchIncome.text = "+" + gold;
        }
        private void UpdateShardDisplay(int shards, int wave)
        {
            shardReward.SetActive(shards != 0);
            shardText.text = "Wave " + (wave + 1) + " Completed";
            shardIncome.text = "+" + shards;
        }
    }
}