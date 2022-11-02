using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.GameDesign;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using GameDesign;
using GameMath;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

using Debug = UnityEngine.Debug;


[TestFixture]
public class BigIntOperationsTests : ZenjectUnitTestFixture
{        
    BigInteger _bigNumber = new BigInteger(1868583826484823036);
    BigInteger _maxDouble = new BigInteger(double.MaxValue);
        
    [Test, RequiresPlayMode(false)]
    public void RandomBigIntListTest()
    {
        var repeats = 20;
        var minValue = new BigInteger(20);
        var maxValue = _bigNumber;
        var range = new List<BigInteger>();
        for(BigInteger value = minValue; value < maxValue; value = value.multiplyByFraction(1.07)) range.Add(value);        
        range.AsParallel().ToList().ForEach(entry => GenerateIntList(entry, repeats));
    }
    
    void GenerateIntList(BigInteger score, int repeats)
    {
        (int Min, int Max) targetCountRange = (1, MaxTargerCount(score)); 
        var targetCount = GlobalRandom.RandomIntInclusive(targetCountRange.Min, targetCountRange.Max);
        Enumerable.Range(0, repeats).ToList().ForEach(entry => GenerateAndCheck(score, targetCount));
    }
    
    void GenerateAndCheck(BigInteger score, int targetCount)
    {
        var result = RandomBigIntListWithSetSum.Generate(score, targetCount, spreadDeviation: (0.2f, 0.85f));
        if(result.Aggregate(BigInteger.Add) != score)
            throw new System.Exception($"RandomBigIntListWithSetSum result produces wrong sum: \n{result.Aggregate(BigInteger.Add)} \n{score}");
    }
    
    
    int MaxTargerCount(System.Numerics.BigInteger score) // HACK copied from ArrowsRunthroughFactory
    {
        var value = (score > 20) ? 20 : (int)score - 1;        
        return value;
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void BigIntegerFractionalPowerSpeed()
    {        
        var value = new BigInteger(1868583826484823036);
        var power = 1.8;
        Measure.Method(() => value.PowFractional(power))
            .WarmupCount(5)
            .MeasurementCount(25)
            .Run();
    }
    
    [Test, RequiresPlayMode(false)]
    public void BigIntegerFractionalPowerRightResult()
    {
        var values = Enumerable.Range(20, 150);
        var powers = new List<float>();
        for(float i = 0.5f; i <= 2.5f; i+=0.25f) powers.Add(i);
        var mathResults = values.Select(value => powers.Select(power => new BigInteger(Mathf.RoundToInt(Mathf.Pow(value, power))))).SelectMany(x => x).ToList();
        var methodResults = values.Select(value => powers.Select(power => new BigInteger(value).PowFractional(power))).SelectMany(x => x).ToList();
        foreach(var index in Enumerable.Range(0, mathResults.Count()))
        {
            var difference = BigInteger.Abs(mathResults.ElementAt(index) - methodResults.ElementAt(index));
            var percent = (difference*100)/mathResults.ElementAt(index);
            Assert.That(difference <=1 || percent <= 2, $"Difference: {percent}%, values: {mathResults.ElementAt(index)} | {methodResults.ElementAt(index)}");
        }
    }
    
    [Test, RequiresPlayMode(false)]
    public void BigIntegerFractionalPowerHighRange()
    {        
        var endHighRange = _maxDouble*10000;        
        var highRange = new List<BigInteger>();        
        var positivePowers = new List<float>();
        for(BigInteger i = _maxDouble; i <= endHighRange; i*=2) highRange.Add(i);
        for(float i = 1.0f; i <= 2.5f; i+=0.25f) positivePowers.Add(i);
        var highResults = highRange.Select(value => positivePowers.Select(power => value.PowFractional(power))).ToList();
        foreach(var index in Enumerable.Range(0, highResults.Count()))
            foreach(var value in highResults.ElementAt(index))
                Assert.That(value >= highRange.ElementAt(index), $"Powered lower than original: {value} | {highRange.ElementAt(index)}");
    }
}
