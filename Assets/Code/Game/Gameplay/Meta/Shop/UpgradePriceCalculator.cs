using System;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Meta.Shop
{
    public struct UpgradePriceCalculator : IItemPriceCalculator
    {
        const int _cacheSize = 250;
        BigInteger[] _levelPriceCache;//= new BigInteger[_cacheSize];
        UpgradePriceFormula _formula;
        
        public UpgradePriceCalculator(UpgradePriceFormula formula)
        {
            _formula = formula ?? throw new ArgumentNullException(nameof(formula));
            _levelPriceCache = CreateCache(_formula);
        }
        
        static BigInteger[] CreateCache(UpgradePriceFormula formula)
        {
            // for(int i = 0; i < _cacheSize; i++) _levelPriceCache[i] = _formula.Evaluate(i);
            return Enumerable.Range(0, _cacheSize)
                .AsParallel()
                .Select(entry => formula.Evaluate(entry))
                .OrderBy(entry => entry)
                .ToArray();
        }
        
        public BigInteger GetPrice(PricingContext context)
        {
            // if(context.ItemLevel < _cacheSize)
            //     return _levelPriceCache[context.ItemLevel];
            // return _formula.Evaluate(context.ItemLevel);
            return GetPrice(context.ItemLevel);
        }
        
        public BigInteger GetPrice(int currentLevel)
        {
            if(currentLevel < _cacheSize)
                return _levelPriceCache[currentLevel];
            return _formula.Evaluate(currentLevel);
        }
    }
}