using Game.Gameplay.Meta;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class UserContextConverter : IContextProvider
    {
        UpgradeContext _upgrades;
        
        public UserContextConverter(IUserContextRetriver userContextRetriver)
        {
            _upgrades = userContextRetriver.Context.Upgrades;
        }
        
        public SequenceContext getContext()
        {                                 
            var initialValue = _upgrades.InitialArrowCount;
            
            var numberOfOperations = GetNumberOfOperationsFromUpgrades();
            
            var length = GetTrackLength(numberOfOperations);
            
            var context = new SequenceContext(length, initialValue, numberOfOperations);
            return context;
        }
        
        int GetNumberOfOperationsFromUpgrades()
        {
            return 5 + _upgrades.CrossbowLevel/2;
        }
        
        int GetTrackLength(int numberOfGates)
        {
            const int unitsPerGate = 30;
            return unitsPerGate * numberOfGates;
        }
    }
}