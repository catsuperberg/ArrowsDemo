using Game.Gameplay.Meta.UpgradeSystem;
using System.Collections.Generic;
using System.Numerics;

namespace Game.GameDesign
{
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
        public PlayerContext Context {get => _context;}
        protected PlayerContext _context;
        
        public readonly PlayerActors Actors;
        public string HeaderString {get; private set;}
        
        protected int _rewardsCount = 0;
        
        public VirtualPlayer(PlayerActors actors)
        {
            Actors = actors;            
            Reset();
            ComposeHeaderString();
        }       
        
        public void Reset()
        {
            var upgrades = new UpgradeContext();
            var sequenceContext = SimulationSequnceContextProvider.DefaultContext;
            _context = new PlayerContext(upgrades, sequenceContext, 0, new List<int>(), BuyerType.Invalid);
            _rewardsCount = 0;
        }
        
        void ComposeHeaderString()
        {
            HeaderString = "Player behaviour: ";
            HeaderString += $"Gates: {Actors.GateSelector.Grade.ToString()} ";
            HeaderString += $"AdSelector: {Actors.AdSelector.GetType().Name} ";
            HeaderString += $"UpgradeBuyer: {Actors.UpgradeBuyer.GetType().Name} ";
        }
        
        virtual public void BuyUpgrades()
        {
            var results = Actors.UpgradeBuyer.BuyAll(_context.Upgrades, _context.CurrencieCount);
            var sequenceProvider = new SimulationSequnceContextProvider(_context);
            var sequence = sequenceProvider.GetContext();
            _context = new PlayerContext(
                _context, new List<int>{results.UpgradesBought}, buyerType: results.TypeUsed, 
                upgrades: results.NewUpgrades, sequenceContext: sequence, currencieCount: results.PointsLeft);            
        }
        
        public void RecieveReward(BigInteger rewardAmount)
        {
            _context = new PlayerContext(_context, new List<int>(), currencieCount: _context.CurrencieCount + rewardAmount);
            _rewardsCount++;
        }      
    }
    
    public class AverageVirtualPlayer : VirtualPlayer
    {
        CountBuyer _buyer;
        UpgradeAtRunIndexCalculator _upgradeCountCalculator;
        
        public AverageVirtualPlayer(PlayerActors actors, CountBuyer buyer, UpgradeAtRunIndexCalculator upgradeCountCalculator) : base(actors)
        {
            _buyer = buyer ?? throw new System.ArgumentNullException(nameof(buyer));
            _upgradeCountCalculator = upgradeCountCalculator ?? throw new System.ArgumentNullException(nameof(upgradeCountCalculator));
        }
        
        override public void BuyUpgrades()
        {
            var results = _buyer.BuyAll(_context.Upgrades, _upgradeCountCalculator.GetUpgradeCount(_rewardsCount));
            var sequenceProvider = new SimulationSequnceContextProvider(_context);
            var sequence = sequenceProvider.GetContext();
            base._context = new PlayerContext(
                _context, new List<int>{results.UpgradesBought}, buyerType: results.TypeUsed, 
                upgrades: results.NewUpgrades, sequenceContext: sequence, currencieCount: results.PointsLeft);   
        }
    }
}