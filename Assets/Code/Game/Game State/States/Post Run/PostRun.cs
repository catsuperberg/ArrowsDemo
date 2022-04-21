using Game.Gameplay.Realtime;
using Game.GameState.Context;
using System;
using System.Numerics;
using UnityEngine;

namespace Game.GameState
{    
    public class PostRun : MonoBehaviour, IPostRun
    {
        [SerializeField]
        PreAdTease _preAdTease;
        [SerializeField]
        RewardApplier _rewardApplier;
                
        public GameObject GO {get {return this.gameObject;}}
        public PostRunContext Context {get; private set;}
        public event EventHandler OnProceedToNextState;   
        
        void OnDestroy()
        {
            _rewardApplier.ApplyReward(Context.SelectedReward);
        }
        
        public void Initialize(RunFinishContext finishContext, BigInteger PlayerCoins)
        {
            _preAdTease.Initialize(finishContext, PlayerCoins);
            _preAdTease.OnCoinsTransferred += RewardSelected;
        }
        
        void RewardSelected(object caller, EventArgs args)
        {
            Context = new PostRunContext(_preAdTease.RewardThatPlayerGets, _preAdTease.AdRequested, 
                restartInsteadOfMenu: false);
            HideChildObjects();
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
        
        void HideChildObjects()
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
        }
        
        // public void SubscribeActualReward(IFinishNotification objectToWaitFor)
        // {
        //     _rewardApplier.SubscribeActualReward(objectToWaitFor, Context.SelectedReward);
        // }
        
        // public void AddReward()
        // {
        //     _rewardApplier.ApplyReward(Context.SelectedReward);
        // }
        
    }
}