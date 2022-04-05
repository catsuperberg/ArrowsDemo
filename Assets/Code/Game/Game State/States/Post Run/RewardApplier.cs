using DataManagement;
using Game.Gameplay.Meta.Curencies;
using System;
using System.Numerics;
using UnityEngine;
using Zenject;

namespace Game.GameState
{    
    public class RewardApplier : MonoBehaviour
    {
        IRegistryAccessor _coinDataAccessor;
        BigInteger _rewardToAdd;
        IFinishNotification _objectToWaitFor;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryAccessor not provided to " + this.GetType().Name);
            
            _coinDataAccessor = registryAccessor;
        }
        
        public void SubscribeActualReward(IFinishNotification objectToWaitFor, BigInteger RewardToGive)
        {
            gameObject.transform.SetParent(null);
            _rewardToAdd = RewardToGive;
            _objectToWaitFor = objectToWaitFor;
            _objectToWaitFor.OnFinished += AddReward;
        }
        
        void AddReward(object sender, EventArgs args)
        {            
            _objectToWaitFor.OnFinished -= AddReward;
            _objectToWaitFor = null;
            ApplyReward(_rewardToAdd);
                
            Destroy(gameObject);
        }
        
        public void ApplyReward(BigInteger RewardToGive)
        {
            _coinDataAccessor.ApplyOperationOnRegisteredField(typeof(CurenciesContext), 
                nameof(CurenciesContext.CommonCoins), OperationType.Increase, RewardToGive.ToString());
        }
    }
}