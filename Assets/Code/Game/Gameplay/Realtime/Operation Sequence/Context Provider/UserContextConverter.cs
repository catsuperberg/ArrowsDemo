using DataManagement;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using Zenject;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class UserContextConverter : IContextProvider
    {
        IRegistryValueReader _registryReader;
        
        (int min, int max) speedRange = (20, 50);
        
        public UserContextConverter([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryReader)
        {            
            if(registryReader == null)
                throw new ArgumentNullException("IRegistryValueReader isn't provided to " + this.GetType().Name);
            
            _registryReader = registryReader;  
        }
        
        public SequenceContext GetContext()
        {                                 
            var initialValue = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.InitialArrowCount)));
            var arrowLevel = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.ArrowLevel)));             
            var crossbowLever = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.CrossbowLevel)));
            
            var speed = MathUtils.MathClamp(speedRange.min+arrowLevel, speedRange.min, speedRange.max);
            var overflowingArrowLevels = (speed < speedRange.max) ? 0 : arrowLevel - (speedRange.max - speedRange.min);
            var numberOfOperations = GetNumberOfOperationsFromUpgrades(crossbowLever, overflowingArrowLevels);            
            var length = GetTrackLength(numberOfOperations);
                        
            
            var context = new SequenceContext(length, initialValue, numberOfOperations, speed);
            return context;
        }
        
        int GetNumberOfOperationsFromUpgrades(int crossbowLever, int overflowingArrowLevels)
        {
            var numberWithPart = 5 + (float)crossbowLever/2 + (float)overflowingArrowLevels/3;
            var intBase = (int)MathF.Floor(numberWithPart);
            var chancePart = numberWithPart - intBase;
            return (DecideOnAdditionalOperation(chancePart)) ? intBase + 1 : intBase;
        }
        
        bool DecideOnAdditionalOperation(float chance)
        {
            return chance >= GlobalRandom.RandomDouble();
        }
        
        int GetTrackLength(int numberOfGates)
        {
            const int unitsPerGate = 30;
            return unitsPerGate * numberOfGates;
        }
    }
}