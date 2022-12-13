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
        public IReadOnlyCollection<BuyerType> BuyerTypePerIteration {get => _buyerTypePerIteration.AsReadOnly();}
        List<int> _upgradesPerIteration;
        List<BuyerType> _buyerTypePerIteration = new List<BuyerType>();

        public PlayerContext(
            UpgradeContext upgrades, SequenceContext sequenceContext, 
            BigInteger currencieCount, IEnumerable<int> upgradesPer, BuyerType buyerType)
        {
            Upgrades = upgrades ?? throw new System.ArgumentNullException(nameof(upgrades));
            SequenceContext = sequenceContext;
            CurrencieCount = currencieCount;
            _upgradesPerIteration = upgradesPer.ToList();
            if(buyerType != BuyerType.Invalid)
                _buyerTypePerIteration.Add(buyerType);
        }
        
        public PlayerContext(
            PlayerContext original, IEnumerable<int> upgradesPer, BuyerType buyerType = BuyerType.Invalid,
            UpgradeContext upgrades = null, SequenceContext? sequenceContext = null, BigInteger? currencieCount = null)
        {
            Upgrades = upgrades ?? original.Upgrades;
            SequenceContext = sequenceContext ?? original.SequenceContext;
            CurrencieCount = currencieCount ?? original.CurrencieCount;
            _upgradesPerIteration = original.UpgradesPerIteration.ToList();
            _upgradesPerIteration.AddRange(upgradesPer);
            if(buyerType != BuyerType.Invalid)
                _buyerTypePerIteration.Add(buyerType);
        }
    }
}