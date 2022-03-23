using DataManagement;
using Game.Gameplay.Meta;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class UserContextConverter : IContextProvider
    {
        IRegistryValueReader _registryReader;
        
        public UserContextConverter(IRegistryValueReader registryReader)
        {            
            if(registryReader == null)
                throw new ArgumentNullException("IRegistryValueReader isn't provided to " + this.GetType().Name);
            
            _registryReader = registryReader;  
        }
        
        public SequenceContext getContext()
        {                                 
            var initialValue = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.InitialArrowCount)));
            
            var numberOfOperations = GetNumberOfOperationsFromUpgrades();
            
            var length = GetTrackLength(numberOfOperations);
            
            var speed = 35;
            
            var context = new SequenceContext(length, initialValue, numberOfOperations, speed);
            return context;
        }
        
        int GetNumberOfOperationsFromUpgrades()
        {
            var crossbowLever = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.CrossbowLevel)));
            return 5 + crossbowLever/2;
        }
        
        int GetTrackLength(int numberOfGates)
        {
            const int unitsPerGate = 30;
            return unitsPerGate * numberOfGates;
        }
    }
}