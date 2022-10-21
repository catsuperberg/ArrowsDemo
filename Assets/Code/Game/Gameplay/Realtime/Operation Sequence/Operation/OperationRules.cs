using GameDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{      
    public class OperationRules : IOperationRules
    {
        const int _startInitValue = 1;
        const int _minInitlessValue = 21; // HACK because of max subtract 10 and min devision 2, after 20 same results regardless of init value        
        public int MinInitless {get => _minInitlessValue;}
        
        public static readonly Dictionary<int, BestChoice> _resultCache;
        public static readonly Dictionary<int, BestChoice> _fastResultCache;
        
        
        static OperationRules()
        {
            var types = Enumerable.Range((int)Operation.First, (int)Operation.Last);
            var baseParams = OperationValueParametersFactory.BaseValueParameters;
            var operationsWithValues = types
                .Select(type => (Operation)type)
                .Select(type => Enumerable.Range(baseParams[type].min, (baseParams[type].max - baseParams[type].min)+1)
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
        
        /// <summary> initialValue can't be higher or equal _minInitlessValue </summary>
        public BestChoice ChooseBest(int leftIdentifier, int rightIdentifier, BigInteger initialValue)
            => _resultCache[IdHash(leftIdentifier, rightIdentifier, (int)initialValue)];
            
        public BestChoice ChooseFastBest(int leftIdentifier, int rightIdentifier)
            => _fastResultCache[FastIdHash(leftIdentifier, rightIdentifier)];
            
        static int IdHash(int leftIdentifier, int rightIdentifier, int initialValue)
            => (initialValue << 16) ^ (leftIdentifier << 8) ^ rightIdentifier;
            
        static int FastIdHash(int leftIdentifier, int rightIdentifier)
            => (leftIdentifier << 8) ^ rightIdentifier;
            
        
        
        OperationProbabilitiesFactory _probabilities;        
        public IReadOnlyDictionary<Operation, int> OperationFrequencies {get => _probabilities.LoadedFrequencies;}
        public IReadOnlyDictionary<Operation, int> OperationRepeats(int count)
            => OperationProbabilitiesFactory.GetRepeatsForCertainCount(OperationFrequencies, count);
        
        readonly IReadOnlyDictionary<Operation, (int min, int max, float coeff)> _valueParameters;
        
        public OperationRules(OperationProbabilitiesFactory probabilities, OperationValueParametersFactory parameters)
        {
            _probabilities = probabilities ?? throw new ArgumentNullException(nameof(probabilities));
            _valueParameters = parameters?.ValueParameters ?? throw new ArgumentNullException(nameof(parameters));
        }              
                  
        public int GetValueForType(Operation type, float randStdNormal) 
        {
            var parameters = _valueParameters[type];
            return GetValue(parameters.min, parameters.max, parameters.coeff, randStdNormal);
        }
        
        static int GetValue(int min, int max, float coeff, float randStdNormal) 
        {                           
            var mean = (max-min)*coeff + min;
            var stdDev = 3; 
            var randNormal = FastRound(mean + stdDev * randStdNormal); 
            
            return Math.Clamp(randNormal, min, max);
        }
        
        static int FastRound(float value)
            => (int)(value + 0.5d);
                     
                    
        
        static readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> _delegates = new Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>>(){
                    {Operation.Add, Addition},
                    {Operation.Subtract, Subtraction},
                    {Operation.Multiply, Multiplication},
                    {Operation.Divide, Division},
                    {Operation.Blank, Keep}}; 
        
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action)
            => GetTypeDelegate(action);
            
        public static Func<BigInteger, BigInteger, BigInteger> GetTypeDelegate(Operation action)
            => _delegates[action];
            
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