using System;
using System.Numerics;
using ExtensionMethods;

namespace Game.Gameplay.Realtime.GameplayComponents
{    
    public class RewardCalculator
    {
        public BigInteger Reward {get; private set;} = 0;
        
        public event EventHandler OnRewardChanged;
        
        public RewardCalculator(IMultiplierEventNotifier notifier)
        {
            notifier.OnMultiplierEvent += MultiplierEventRecieved;
        }
        
        public void IncreaseReward(BigInteger amount)
        {
            Reward += amount;
            OnRewardChanged?.Invoke(this, EventArgs.Empty);            
        }
        
        void MultiplierEventRecieved(object sender, MultiplierEventArgs arguments)
        {
            Reward = Reward.multiplyByFractionFast(arguments.Multiplier);
            OnRewardChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}