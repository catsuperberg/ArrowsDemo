using Game.Gameplay.Realtime;
using Game.GameState.Context;
using System;
using System.Numerics;
using UnityEngine;

namespace Game.GameState
{    
    public class PostRun : MonoBehaviour
    {
        [SerializeField]
        PreAdTease _preAdTease;
        [SerializeField]
        RewardApplier _rewardApplier;
                
        public PostRunContext Context {get; private set;}
        public event EventHandler OnProceedToNextState;   
        
        public void Initialize(RunFinishContext finishContext, BigInteger PlayerCoins)
        {
            _preAdTease.Initialize(finishContext, PlayerCoins);
            _preAdTease.OnCoinsTransferred += RewardSelected;
        }
        
        void RewardSelected(object caller, EventArgs args)
        {
            Context = new PostRunContext(_preAdTease.RewardThatPlayerGets, _preAdTease.AdRequested);
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
        
        public void SubscribeActualReward(IFinishNotification objectToWaitFor)
        {
            _rewardApplier.SubscribeActualReward(objectToWaitFor, Context.SelectedReward);
        }
        
        public void AddReward()
        {
            _rewardApplier.ApplyReward(Context.SelectedReward);
        }
    }
}