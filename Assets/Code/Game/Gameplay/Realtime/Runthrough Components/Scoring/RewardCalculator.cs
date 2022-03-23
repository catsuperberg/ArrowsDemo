using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using System;
using System.Numerics;
using UnityEngine;
using ExtensionMethods;

namespace Game.Gameplay.Realtime.GameplayComponents
{    
    public class RewardCalculator
    {
        public BigInteger Reward {get; private set;} = 0;
        
        public RewardCalculator(IMultiplierEventNotifier notifier)
        {
            notifier.OnMultiplierEvent += MultiplierEventRecieved;
        }
        
        public void IncreaseReward(BigInteger amount)
        {
            Reward += amount;
        }
        
        void MultiplierEventRecieved(object sender, MultiplierEventArgs arguments)
        {
            Reward = Reward.multiplyByFraction(arguments.Multiplier);
        }
    }
}