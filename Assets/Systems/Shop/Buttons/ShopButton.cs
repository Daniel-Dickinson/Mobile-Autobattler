using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class ShopButton : CondionalButton
    {
        [Header("Conditional")]
        [SerializeField] private PlayerState state;
        [SerializeField] private int cost = 1;

        //Access
        public int Cost
        {
            get { return cost; }
            set 
            { 
                cost = value;
                QueryState();
            }
        }

        //Mono
        protected void Awake()
        {
            state.OnGoldChange += QueryState;

            QueryState();
        }
        private void OnDestroy()
        {
            state.OnGoldChange -= QueryState;
        }

        private void QueryState()
        {
            SetState(state.Gold >= cost);
        }
    }
}