using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Meta.UpgradeSystem;
using System;

namespace Game.GameDesign
{
    public class SimulationSequnceContextProvider
    {
        readonly PlayerContext _playerContext;

        public SimulationSequnceContextProvider(PlayerContext playerContext)
        {
            _playerContext = playerContext ?? throw new ArgumentNullException(nameof(playerContext));
        }
        
        public SequenceContext GetContext()
            => UserContextConverter.GetContext(_playerContext.Upgrades);
        
        public static SequenceContext DefaultContext
            => UserContextConverter.GetContext(new UpgradeContext());
    }
}