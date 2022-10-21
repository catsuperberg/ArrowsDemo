using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.Gameplay.Meta.Shop;
using NUnit.Framework;
using System;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.TestTools;

public class UpgradeFormulaTests
{
    const string _testJsonFilePath = "Assets/Code/Game/Game Design/Game Design Tests/Resources/formulaTest.json";
    
    [Test, RequiresPlayMode(false)]
    public void JsonFileDeserialization()
    {        
        var createdFormula = new UpgradePriceFormula(new BigInteger(200), 1.1, 0.013, 1.2);
        JsonFile.SaveAsJson(createdFormula, _testJsonFilePath);
        createdFormula = null;
        var deserializedFormula = JsonFile.GetObjectFromFile<UpgradePriceFormula>(_testJsonFilePath);
        Assert.That(_testJsonFilePath, Does.Exist);
        CompareToOriginalCalculations(deserializedFormula);
        // File.Delete(_testJsonFilePath);
    }
        
    
    [Test, RequiresPlayMode(false)]
    public void SerializationJsonNotEmpty()
    {
        var priceFormula = new UpgradePriceFormula(new BigInteger(200), 1.1, 0.013, 1.2);
        var json = JsonFile.ToJson(priceFormula);
        Assert.That(json, Is.Not.Null);
        Assert.That(json, Is.Not.EqualTo("[]"));
        Assert.That(json, Is.Not.EqualTo("{}"));
        Debug.Log(json);
    }
           
        
    [Test, RequiresPlayMode(false)]
    public void FomulaAndOriginalCalculationTheSame()
    {
        var priceFormula = new UpgradePriceFormula(new BigInteger(200), 1.1, 0.013, 1.2);
        CompareToOriginalCalculations(priceFormula);
    }
    
    void CompareToOriginalCalculations(UpgradePriceFormula formula)
    {
        var valuesCount = 150;
        var originalPrices = Enumerable
            .Range(1, valuesCount)
            .Select(OriginalArrowLevelPriceCalculation);
        var formulaPrices = Enumerable
            .Range(1, valuesCount)
            .Select(formula.Evaluate);
        Assert.That(formulaPrices, Is.EquivalentTo(originalPrices));
    }
    
    BigInteger OriginalArrowLevelPriceCalculation(int level)
    {
        var initialPrice = new BigInteger(200);
        var power = 1.1 + (0.013 * Math.Pow(level, 1.2));
        var price = initialPrice.PowFractional(power);
        return price;
    }
}
