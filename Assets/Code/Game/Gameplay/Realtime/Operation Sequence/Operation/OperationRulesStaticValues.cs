using Game.GameDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{      
    public class OperationRulesStaticValues
    {
        const int _startInitValue = 1;
        public readonly static BigInteger MinInitlessValue = new BigInteger(21); // HACK because of max subtract 10 and min devision 2, after 20 same results regardless of init value        
        public BigInteger MinInitless {get => MinInitlessValue;}
        
        public readonly BestChoice[] ResultCache;
        public readonly BestChoice[] FastResultCache;           
        
        public readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> _delegates = new Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>>(){
                    {Operation.Add, BigInteger.Add},
                    {Operation.Subtract, BigInteger.Subtract},
                    {Operation.Multiply, BigInteger.Multiply},
                    {Operation.Divide, BigInteger.Divide}};         
        
        public OperationRulesStaticValues()
        {
            var types = Enumerable.Range((int)Operation.First, (int)Operation.Last);
            var baseParams = OperationValueParametersFactory.BaseValueParameters;
            var operationsWithValues = types
                .Select(type => (Operation)type)
                .Select(type => Enumerable.Range(baseParams[type].min, (baseParams[type].max - baseParams[type].min)+1)
                    .Select(entry => type != Operation.Blank ? new OperationInstance(type, entry, _delegates[type]) : OperationInstance.blank))
                .SelectMany(entry => entry);
            var allPossiblePairs = new List<(OperationInstance left, OperationInstance right)>();
            foreach(var leftOperation in operationsWithValues)
                foreach(var rightOperation in operationsWithValues)
                    allPossiblePairs.Add((leftOperation, rightOperation));
            var initValues = Enumerable.Range(_startInitValue, (((int)MinInitlessValue-1) - _startInitValue)+1);       
            var cache = new Dictionary<int, BestChoice>();   
            foreach(var value in initValues)
                foreach(var pair in allPossiblePairs)
                    cache.TryAdd(IdHash(pair.left.Identifier, pair.right.Identifier, value), CalculateBestOperation(pair.left, pair.right, value));
            var highestKey = cache.Keys.Max();
            ResultCache = new BestChoice[highestKey+1];
            foreach(var choice in cache)
                ResultCache[choice.Key] = choice.Value;
            
            var fastCache = new Dictionary<int, BestChoice>(); 
            foreach(var pair in allPossiblePairs)
                    fastCache.Add(FastIdHash(pair.left.Identifier, pair.right.Identifier), CalculateBestOperation(pair.left, pair.right, (int)MinInitlessValue));
            var highestFastKey = cache.Keys.Max();
            FastResultCache = new BestChoice[highestFastKey+1];
            foreach(var choice in fastCache)
                FastResultCache[choice.Key] = choice.Value;
        }
        
        static BestChoice CalculateBestOperation(OperationInstance left, OperationInstance right, int initValue)
        {
            var leftResult = left.Perform(initValue);
            var rightResult = right.Perform(initValue);
            if(leftResult > rightResult)
                return BestChoice.Left;
            if(rightResult > leftResult)
                return BestChoice.Right;
            return BestChoice.Both;
        }
        
        public static int IdHash(int leftIdentifier, int rightIdentifier, int initialValue)
            => (initialValue << 16) ^ (leftIdentifier << 8) ^ rightIdentifier;
            
        public static int FastIdHash(int leftIdentifier, int rightIdentifier)
            => (leftIdentifier << 8) ^ rightIdentifier; 
    }
}