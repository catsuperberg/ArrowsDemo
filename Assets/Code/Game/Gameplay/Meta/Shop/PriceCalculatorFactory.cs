using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.Gameplay.Meta.UpgradeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.Gameplay.Meta.Shop
{
    public class PriceCalculatorFactory
    {
        const string _formualPrefix = "Formula";
        Dictionary<string, IItemPriceCalculator> _calculators = new Dictionary<string, IItemPriceCalculator>();
        
        IGameFolders _folders;        
                   
        public PriceCalculatorFactory(IGameFolders folders)
        {
            _folders = folders ?? throw new ArgumentNullException(nameof(folders));
            var formulas = LoadAllFormulas();
            _calculators = formulas
                .Select(kvp => new KeyValuePair<string, IItemPriceCalculator>(kvp.Key, new UpgradePriceCalculator(kvp.Value)))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
            
            if(_calculators == null || !_calculators.Any())
                throw new Exception("No calculators were created by PriceCalculatorFactory");
        }
        
        public IItemPriceCalculator GetCalculatorFor(string variableName)
            => _calculators[variableName] ?? throw new Exception($"couldn't find aprorpiet calculator for {variableName}");
        
        Dictionary<string, UpgradePriceFormula> LoadAllFormulas()
        {
            var fileNamesInFolder = Resources.LoadAll<TextAsset>(_folders.ResourcesGameBalance.GetAtResourcesWithNoExtension())
                .Select(entry => entry.name); // HACK can't search resources folder
            var formulaFiles = fileNamesInFolder.Where(name => name.StartsWith(_formualPrefix));
            return formulaFiles.ToDictionary(
                fileName => fileName.Split(_formualPrefix)[1], 
                fileName => JsonFile.LoadFromResources<UpgradePriceFormula>(_folders.ResourcesGameBalance, fileName));
        }
    }
}