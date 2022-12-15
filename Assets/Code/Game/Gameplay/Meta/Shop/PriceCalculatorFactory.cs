using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.GameDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Meta.Shop
{
    public class PriceCalculatorFactory
    {
        Dictionary<string, UpgradePriceFormula> _priceFormulas = new Dictionary<string, UpgradePriceFormula>()
            {
                {"ArrowLevel", new UpgradePriceFormula(200, 1.1, 0.013, 1.2)}, 
                {"CrossbowLevel", new UpgradePriceFormula(500, 1.1, 0.022, 1.2)}, 
                {"InitialArrowCount", new UpgradePriceFormula(300, 1.1, 0.17, 1.2)}, 
            };
        
        const string _formualPrefix = "Formula";
        Dictionary<string, IItemPriceCalculator> _calculators = new Dictionary<string, IItemPriceCalculator>();
        
        IGameFolders _folders;  
        GameBalanceConfiguration _balanceConfiguration;   
                   
        public PriceCalculatorFactory(IGameFolders folders, GameBalanceConfiguration balanceConfiguration)
        {
            _folders = folders ?? throw new ArgumentNullException(nameof(folders));
            _balanceConfiguration = balanceConfiguration ?? throw new ArgumentNullException(nameof(balanceConfiguration));
            
            
            _calculators = FormulasWithBalancingApplied()
                .Select(kvp => new KeyValuePair<string, IItemPriceCalculator>(kvp.Key, new UpgradePriceCalculator(kvp.Value)))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
            
            if(_calculators == null || !_calculators.Any())
                throw new Exception("No calculators were created by PriceCalculatorFactory");
        }
        
        public IItemPriceCalculator GetCalculatorFor(string variableName)
            => _calculators[variableName] ?? throw new Exception($"couldn't find appropriate calculator for {variableName}");
        
        Dictionary<string, UpgradePriceFormula> FormulasWithBalancingApplied()
        {
            // _balanceConfiguration.CheapestUpgradeStartingPrice;
            var cheapestUpgrade = _priceFormulas.OrderBy(entry => entry.Value.BaseValue).First();
            var basePriceCoeff = ((double)_balanceConfiguration.CheapestUpgradeStartingPrice)/((double)cheapestUpgrade.Value.BaseValue);
            
            return _priceFormulas
                .ToDictionary(formula => formula.Key, formula => new UpgradePriceFormula(
                    formula.Value, baseValue: formula.Value.BaseValue.multiplyByFraction(basePriceCoeff), 
                    baseIncrement: formula.Value.BaseIncrement*_balanceConfiguration.PriceIncreaseSteepness,
                    incrementPower: formula.Value.IncrementPower*_balanceConfiguration.LatePriceIncreaseSpeedup));
        }
        
        
        
        // Dictionary<string, UpgradePriceFormula> LoadAllFormulas()
        // {
        //     var fileNamesInFolder = Resources.LoadAll<TextAsset>(_folders.ResourcesGameBalance.GetAtResourcesWithNoExtension())
        //         .Select(entry => entry.name); // HACK can't search resources folder
        //     var formulaFiles = fileNamesInFolder.Where(name => name.StartsWith(_formualPrefix));
        //     return formulaFiles.ToDictionary(
        //         fileName => fileName.Split(_formualPrefix)[1], 
        //         fileName => JsonFile.LoadFromResources<UpgradePriceFormula>(_folders.ResourcesGameBalance, fileName));
        // }
    }
}