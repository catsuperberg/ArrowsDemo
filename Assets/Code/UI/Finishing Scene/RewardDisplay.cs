using Game.Gameplay.Realtime.GameplayComponents;
using System;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RewardDisplay : MonoBehaviour
    {        
        static BigInteger _zero = BigInteger.Zero; //HACK original properties construct new BigInteger every time
        
        [SerializeField]
        private TMP_Text RewardAmountString;
        
        RewardCalculator _reward;
        
        public BigInteger Count {get; private set;} = _zero;
                
        public void Initialize(RewardCalculator reward)
        {
            if(reward == null)
                throw new ArgumentNullException("RewardCalculator isn't provided to" + this.GetType().Name);
                
            _reward = reward;
            _reward.OnRewardChanged += RewardUpdated;
        }      
        
        // void OnDestroy()
        // {            
        //     _reward.OnRewardChanged -= RewardUpdated;
        // }  
        
        void RewardUpdated(object caller, EventArgs args)
        {
            UpdateCount();
        }
        
        void UpdateCount()
        {
            Count = _reward.Reward;
            UpdateAppearance();
        }        
        
        void UpdateAppearance()
        {
            RewardAmountString.text = Count.ToString("N0");            
        }
    }
}