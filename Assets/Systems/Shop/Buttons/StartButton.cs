using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TwoBears.Persistance;

namespace TwoBears.Shop
{
    public class StartButton : CondionalButton
    {
        [Header("Conditional")]
        public PlayerState state;

        //Mono
        protected override void Awake()
        {
            base.Awake();
            
            state.OnFormationChange += QueryState;

            QueryState();
        }
        private void OnDestroy()
        {
            state.OnFormationChange -= QueryState;
        }

        private void QueryState()
        {
            SetState(state.UnitCount > 0);
        }
    }
}