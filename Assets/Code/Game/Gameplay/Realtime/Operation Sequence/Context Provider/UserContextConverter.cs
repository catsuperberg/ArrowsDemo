using DataManagement;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using Zenject;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class UserContextConverter : ISequenceContextProvider
    {
        IRegistryValueReader _registryReader;
                
        static (int min, int max) _speedRange = (20, 50);
        static int _unitLengthPerGate = 30;
        
        public UserContextConverter([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryReader)
        {            
            if(registryReader == null)
                throw new ArgumentNullException("IRegistryValueReader isn't provided to " + this.GetType().Name);
            
            _registryReader = registryReader;  
        }
        
        public SequenceContext GetContext()
        {                                      
            var crossbowLever = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.CrossbowLevel)));
            var arrowLevel = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.ArrowLevel)));        
            var initialValue = Convert.ToInt32(_registryReader.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.InitialArrowCount)));
            return GetContext(new UpgradeContext(crossbowLever, arrowLevel, initialValue));
        }
        
        
        public static SequenceContext GetContext(UpgradeContext upgrades)
        {                                   
            var crossbowLever = upgrades.CrossbowLevel;
            var arrowLevel = upgrades.ArrowLevel;           
            var initialValue = upgrades.InitialArrowCount;
            
            var speed = MathUtils.MathClamp(_speedRange.min+arrowLevel, _speedRange.min, _speedRange.max);
            var overflowingArrowLevels = (speed < _speedRange.max) ? 0 : arrowLevel - (_speedRange.max - _speedRange.min);
            var numberOfOperations = GetNumberOfOperationsFromUpgrades(crossbowLever, overflowingArrowLevels);            
            var length = GetTrackLength(numberOfOperations);
                        
            
            var context = new SequenceContext(length, initialValue, numberOfOperations, speed);
            return context;
        }
        
        static int GetNumberOfOperationsFromUpgrades(int crossbowLever, int overflowingArrowLevels)
        {
            var numberWithPart = 5 + (float)crossbowLever/2 + (float)overflowingArrowLevels/3;
            var intBase = (int)MathF.Floor(numberWithPart);
            var chancePart = numberWithPart - intBase;
            return (DecideOnAdditionalOperation(chancePart)) ? intBase + 1 : intBase;
        }
        
        static bool DecideOnAdditionalOperation(float chance)
            => chance >= GlobalRandom.RandomDouble();
        
        static int GetTrackLength(int numberOfGates)
            => _unitLengthPerGate * numberOfGates;
    }
}