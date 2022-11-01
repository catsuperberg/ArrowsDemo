using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.GameDesign
{
    public class PlayerContext
    {
        public readonly UpgradeContext Upgrades;
        public readonly SequenceContext SequenceContext;
        public readonly BigInteger CurrencieCount;

        public PlayerContext(UpgradeContext upgrades, SequenceContext sequenceContext, BigInteger currencieCount)
        {
            Upgrades = upgrades ?? throw new System.ArgumentNullException(nameof(upgrades));
            SequenceContext = sequenceContext ?? throw new System.ArgumentNullException(nameof(sequenceContext));
            CurrencieCount = currencieCount;
        }
        
        public PlayerContext(PlayerContext original, UpgradeContext upgrades = null, SequenceContext sequenceContext = null, BigInteger? currencieCount = null)
        {
            Upgrades = upgrades ?? original.Upgrades;
            SequenceContext = sequenceContext ?? original.SequenceContext;
            CurrencieCount = currencieCount ?? original.CurrencieCount;
        }
    }
    
    public struct PlayerActors
    {        
        public readonly GateSelector GateSelector;
        public readonly IAdSelector AdSelector;
        public readonly IUpgradeBuyer UpgradeBuyer;

        public PlayerActors(GateSelector gateSelector, IAdSelector adSelector, IUpgradeBuyer upgradeBuyer)
        {
            GateSelector = gateSelector ?? throw new System.ArgumentNullException(nameof(gateSelector));
            AdSelector = adSelector ?? throw new System.ArgumentNullException(nameof(adSelector));
            UpgradeBuyer = upgradeBuyer ?? throw new System.ArgumentNullException(nameof(upgradeBuyer));
        }
    }
    
    public class VirtualPlayer
    {        
        public PlayerContext Context {get; private set;}
        
        public readonly PlayerActors Actors;
        
        
        public VirtualPlayer(PlayerActors actors)
        {
            Actors = actors;
            
            var upgrades = new UpgradeContext();
            var sequenceContext = SimulationSequnceContextProvider.DefaultContext;
            Context = new PlayerContext(upgrades, sequenceContext, 0);
        }
        
        public void BuyUpgrades()
        {
            var results = Actors.UpgradeBuyer.BuyAll(Context.Upgrades, Context.CurrencieCount);
            var sequenceProvider = new SimulationSequnceContextProvider(Context);
            var sequence = sequenceProvider.GetContext();
            Context = new PlayerContext(Context, upgrades: results.NewUpgrades, sequenceContext: sequence, currencieCount: results.PointsLeft);            
        }
        
        public void RecieveReward(BigInteger rewardAmount)
        {
            Context = new PlayerContext(Context, currencieCount: Context.CurrencieCount + rewardAmount);
        }      
    }
}