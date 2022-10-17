using System.Numerics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{  
    public enum BestChoice
    {
        Left,
        Right,
        Both
    }  
    
    public struct OperationRules : IOperationRules
    {
        const int _startInitValue = 1;
        const int _minInitlessValue = 21; // HACK because of max subtract 10 and min devision 2, after 20 same results regardless of init value        
        public int MinInitless {get => _minInitlessValue;}
        
        public static readonly Dictionary<int, BestChoice> _resultCache;
        public static readonly Dictionary<int, BestChoice> _fastResultCache;
        
        static OperationRules()
        {
            var types = Enumerable.Range((int)Operation.First, (int)Operation.Last);
            var operationsWithValues = types
                .Select(type => (Operation)type)
                .Select(type => Enumerable.Range(_range[type].min, (_range[type].max - _range[type].min)+1)
                    .Select(entry => new OperationInstance(type, entry, GetTypeDelegate(type))))
                .SelectMany(entry => entry);
            var allPossiblePairs = new List<(OperationInstance left, OperationInstance right)>();
            foreach(var leftOperation in operationsWithValues)
                foreach(var rightOperation in operationsWithValues)
                    allPossiblePairs.Add((leftOperation, rightOperation));
            var initValues = Enumerable.Range(_startInitValue, ((_minInitlessValue-1) - _startInitValue)+1);       
            var cache = new Dictionary<int, BestChoice>();   
            foreach(var value in initValues)
                foreach(var pair in allPossiblePairs)
                    cache.Add(IdHash(pair.left.Identifier, pair.right.Identifier, value), CalculateBestOperation(pair.left, pair.right, value));
            _resultCache = cache;  
            var fastCache = new Dictionary<int, BestChoice>(); 
            foreach(var pair in allPossiblePairs)
                    fastCache.Add(FastIdHash(pair.left.Identifier, pair.right.Identifier), CalculateBestOperation(pair.left, pair.right, _minInitlessValue));
            _fastResultCache = fastCache;
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
        
        public BestChoice ChooseBest(int leftIdentifier, int rightIdentifier, int initialValue)
            => _resultCache[IdHash(leftIdentifier, rightIdentifier, initialValue)];
            
        public BestChoice ChooseFastBest(int leftIdentifier, int rightIdentifier)
            => _fastResultCache[FastIdHash(leftIdentifier, rightIdentifier)];
            
        static int IdHash(int leftIdentifier, int rightIdentifier, int initialValue)
            => (initialValue << 16) ^ (leftIdentifier << 8) ^ rightIdentifier;
            
        static int FastIdHash(int leftIdentifier, int rightIdentifier)
            => (leftIdentifier << 8) ^ rightIdentifier;
            
        
        
        public int GetValueForType(Operation type, float stdSqrt, float stdSin, float coeff) // coeff should be between 0 and 1
        {               
            // coeff = MathUtils.MathClamp(coeff, 0, 1);
            var range = _range[type];
            var mean = (range.max-range.min)*coeff + range.min;
            var stdDev = 3; 
            var randStdNormal = stdSqrt * stdSin;
            var randNormal = FastRound(mean + stdDev * randStdNormal); 
            
            return Math.Clamp(randNormal, range.min, range.max);
        }
        
        static int FastRound(float value)
            => (int)(value + 0.5d);
            
        public static readonly Dictionary<Operation, (int min, int max)> _range = new Dictionary<Operation, (int min, int max)>(){
                    {Operation.Add, (1, 10)},
                    {Operation.Subtract, (1, 10)},
                    {Operation.Multiply, (2, 4)},
                    {Operation.Divide, (2, 5)},
                    {Operation.Blank, (0, 0)}}; 
                
        // public static readonly Dictionary<int, int> _rangeMin = new Dictionary<int, int>(){
        //             {(int)Operation.Add, 1},
        //             {(int)Operation.Subtract, 1},
        //             {(int)Operation.Multiply, 2},
        //             {(int)Operation.Divide, 2},
        //             {(int)Operation.Blank, 0}}; 
                    
        // public static readonly Dictionary<int, int> _rangeMax = new Dictionary<int, int>(){
        //             {(int)Operation.Add, 10},
        //             {(int)Operation.Subtract, 10},
        //             {(int)Operation.Multiply, 4},
        //             {(int)Operation.Divide, 5},
        //             {(int)Operation.Blank, 0}}; 
        
        public static readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> _delegates = new Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>>(){
                    {Operation.Add, Addition},
                    {Operation.Subtract, Subtraction},
                    {Operation.Multiply, Multiplication},
                    {Operation.Divide, Division},
                    {Operation.Blank, Keep}}; 
        
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action)
            => GetTypeDelegate(action);
            
        public static Func<BigInteger, BigInteger, BigInteger> GetTypeDelegate(Operation action)
            => _delegates[action];
            
        // public (int min, int max) GetRange(int actionInt)
        //     => GetTypeRange(actionInt);
            
        // public static (int min, int max) GetTypeRange(int actionInt)
        //     => (_rangeMin[actionInt], _rangeMax[actionInt]);
        
        static BigInteger Addition(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Add(initialValue, operationValue);
        static BigInteger Subtraction(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Subtract(initialValue, operationValue);
        static BigInteger Multiplication(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Multiply(initialValue, operationValue);
        static BigInteger Division(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Divide(initialValue, operationValue);
        static BigInteger Keep(BigInteger initialValue, BigInteger operationValue)
            => initialValue;
    }
}