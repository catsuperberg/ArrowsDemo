using System;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Meta.Shop
{
    public class UpgradePriceCalculator : IItemPriceCalculator
    {
        const int _cacheSize = 200;
        BigInteger[] _levelPriceCache = new BigInteger[_cacheSize];
        UpgradePriceFormula _formula;
        
        public UpgradePriceCalculator(UpgradePriceFormula formula)
        {
            _formula = formula ?? throw new ArgumentNullException(nameof(formula));
            FillCache();
        }
        
        void FillCache()
        {
            // for(int i = 0; i < _cacheSize; i++) _levelPriceCache[i] = _formula.Evaluate(i);
            _levelPriceCache = Enumerable.Range(0, _cacheSize).AsParallel().Select(entry => _formula.Evaluate(entry)).OrderBy(entry => entry).ToArray();
        }
        
        public BigInteger GetPrice(PricingContext context)
        {
            if(context.ItemLevel < _cacheSize)
                return _levelPriceCache[context.ItemLevel];
            return _formula.Evaluate(context.ItemLevel);
        }
    }
}