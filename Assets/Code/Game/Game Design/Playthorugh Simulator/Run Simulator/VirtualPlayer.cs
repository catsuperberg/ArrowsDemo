using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public class PlayerContext
    {
        public readonly UpgradeContext Upgrades;
        public readonly SequenceContext SequenceContext;
        public readonly BigInteger CurrencieCount;
        public IReadOnlyCollection<int> UpgradesPerIteration {get => _upgradesPerIteration.AsReadOnly();}
        List<int> _upgradesPerIteration;

        public PlayerContext(UpgradeContext upgrades, SequenceContext sequenceContext, BigInteger currencieCount, IEnumerable<int> upgradesPer)
        {
            Upgrades = upgrades ?? throw new System.ArgumentNullException(nameof(upgrades));
            SequenceContext = sequenceContext;// ?? throw new System.ArgumentNullException(nameof(sequenceContext));
            CurrencieCount = currencieCount;
            _upgradesPerIteration = upgradesPer.ToList();
        }
        
        public PlayerContext(
            PlayerContext original, IEnumerable<int> upgradesPer, UpgradeContext upgrades = null, SequenceContext? sequenceContext = null, 
            BigInteger? currencieCount = null)
        {
            Upgrades = upgrades ?? original.Upgrades;
            SequenceContext = sequenceContext ?? original.SequenceContext;
            CurrencieCount = currencieCount ?? original.CurrencieCount;
            _upgradesPerIteration = original.UpgradesPerIteration.ToList();
            _upgradesPerIteration.AddRange(upgradesPer);
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
        public string HeaderString {get; private set;}
        
        
        public VirtualPlayer(PlayerActors actors)
        {
            Actors = actors;
            
            var upgrades = new UpgradeContext();
            var sequenceContext = SimulationSequnceContextProvider.DefaultContext;
            Context = new PlayerContext(upgrades, sequenceContext, 0, new List<int>());
            ComposeHeaderString();
        }
        
        void ComposeHeaderString()
        {
            HeaderString = "Player behaviour: ";
            HeaderString += $"Gates: {Actors.GateSelector.Grade.ToString()} ";
            HeaderString += $"AdSelector: {Actors.AdSelector.GetType().Name} ";
            HeaderString += $"UpgradeBuyer: {Actors.UpgradeBuyer.GetType().Name} ";
        }
        
        public void BuyUpgrades()
        {
            var results = Actors.UpgradeBuyer.BuyAll(Context.Upgrades, Context.CurrencieCount);
            var sequenceProvider = new SimulationSequnceContextProvider(Context);
            var sequence = sequenceProvider.GetContext();
            Context = new PlayerContext(
                Context, new List<int>{results.UpgradesBought}, upgrades: results.NewUpgrades, 
                sequenceContext: sequence, currencieCount: results.PointsLeft);            
        }
        
        public void RecieveReward(BigInteger rewardAmount)
        {
            Context = new PlayerContext(Context, new List<int>(), currencieCount: Context.CurrencieCount + rewardAmount);
        }      
    }
}