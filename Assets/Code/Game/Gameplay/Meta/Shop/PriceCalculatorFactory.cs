using ExtensionMethods;
using Game.GameDesign;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay.Meta.Shop
{
    public class PriceCalculatorFactory
    {
        Dictionary<string, UpgradePriceFormula> _baseFormulas = new Dictionary<string, UpgradePriceFormula>()
            {
                {"ArrowLevel", new UpgradePriceFormula(200, 1.1, 0.013, 1.2)}, 
                {"CrossbowLevel", new UpgradePriceFormula(500, 1.1, 0.022, 1.2)}, 
                {"InitialArrowCount", new UpgradePriceFormula(300, 1.1, 0.017, 1.2)}, 
            };
            
        public readonly Dictionary<string, UpgradePriceFormula> UpgradePriceFormulas;
        
        const string _formualPrefix = "Formula";
        Dictionary<string, IItemPriceCalculator> _calculators = new Dictionary<string, IItemPriceCalculator>();
        
        GameBalanceConfiguration _balanceConfiguration;   
                   
        public PriceCalculatorFactory(GameBalanceConfiguration balanceConfiguration)
        {
            _balanceConfiguration = balanceConfiguration ?? throw new ArgumentNullException(nameof(balanceConfiguration));
            
            UpgradePriceFormulas = FormulasWithBalancingApplied();
            _calculators = UpgradePriceFormulas
                .Select(kvp => new KeyValuePair<string, IItemPriceCalculator>(kvp.Key, new UpgradePriceCalculator(kvp.Value)))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
            
            if(_calculators == null || !_calculators.Any())
                throw new Exception("No calculators were created by PriceCalculatorFactory");
        }
        
        public IItemPriceCalculator GetCalculatorFor(string variableName)
            => _calculators[variableName] ?? throw new Exception($"couldn't find appropriate calculator for {variableName}");
        
        Dictionary<string, UpgradePriceFormula> FormulasWithBalancingApplied()
        {
            var cheapestUpgrade = _baseFormulas.OrderBy(entry => entry.Value.BaseValue).First();
            var basePriceCoeff = ((double)_balanceConfiguration.CheapestUpgradeStartingPrice)/((double)cheapestUpgrade.Value.BaseValue);
            
            return _baseFormulas
                .ToDictionary(formula => formula.Key, formula => new UpgradePriceFormula(
                    formula.Value, baseValue: formula.Value.BaseValue.multiplyByFraction(basePriceCoeff), 
                    baseIncrement: formula.Value.BaseIncrement*_balanceConfiguration.PriceIncreaseSteepness,
                    incrementPower: formula.Value.IncrementPower*_balanceConfiguration.LatePriceIncreaseSpeedup));
        }
    }
}