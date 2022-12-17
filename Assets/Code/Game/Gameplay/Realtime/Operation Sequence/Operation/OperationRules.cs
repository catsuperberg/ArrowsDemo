using Game.GameDesign;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{      
    public class OperationRules : IOperationRules
    {
        static OperationRulesStaticValues _staticValues;
        static OperationRules()
        {
            _staticValues = new OperationRulesStaticValues();
        }
        
        public BigInteger MinInitless {get => MinInitlessValue;}
        public readonly BigInteger MinInitlessValue;
        readonly BestChoice[] _resultCache;
        readonly BestChoice[] _fastResultCache; 
        
        readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> _delegates;

        public OperationRules(OperationProbabilitiesFactory probabilities, OperationValueParametersFactory parameters)
        {
            _probabilities = probabilities ?? throw new ArgumentNullException(nameof(probabilities));
            _valueParameters = parameters?.ValueParameters ?? throw new ArgumentNullException(nameof(parameters));
            
            MinInitlessValue = _staticValues.MinInitless;
            _resultCache = _staticValues.ResultCache;
            _fastResultCache = _staticValues.FastResultCache;
            _delegates = new Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>>(){
                    {Operation.Add, BigInteger.Add},
                    {Operation.Subtract, BigInteger.Subtract},
                    {Operation.Multiply, BigInteger.Multiply},
                    {Operation.Divide, BigInteger.Divide}};
        }      
           
        
        
        /// <summary> initialValue can't be higher or equal _minInitlessValue </summary>
        public BestChoice ChooseBest(int leftIdentifier, int rightIdentifier, BigInteger initialValue)
            => _resultCache[IdHash(leftIdentifier, rightIdentifier, ((int)initialValue))];
            
        public BestChoice ChooseFastBest(int leftIdentifier, int rightIdentifier)
            => _fastResultCache[FastIdHash(leftIdentifier, rightIdentifier)];
            
        public static int IdHash(int leftIdentifier, int rightIdentifier, int initialValue)
            => (initialValue << 16) ^ (leftIdentifier << 8) ^ rightIdentifier;
            
        public static int FastIdHash(int leftIdentifier, int rightIdentifier)
            => (leftIdentifier << 8) ^ rightIdentifier;
            
        
        
        OperationProbabilitiesFactory _probabilities; 
        IReadOnlyDictionary<Operation, int> _repeats = new Dictionary<Operation, int>();     
        int _repeatsCount = 0;  
        public IReadOnlyDictionary<Operation, int> OperationFrequencies {get => _probabilities.LoadedFrequencies;}
        public IReadOnlyDictionary<Operation, int> OperationRepeats(int count)
            => GetRepeats(count);
            
        IReadOnlyDictionary<Operation, int> GetRepeats(int count)
        {
            if(_repeatsCount == count)
                return _repeats;
                
            _repeats = _probabilities.GetRepeatsForCertainCount(count);
            _repeatsCount = count;
            return _repeats;
        }
        
        readonly IReadOnlyDictionary<Operation, (int min, int max, float coeff)> _valueParameters;
                
                  
        public int GetValueForType(Operation type, float randStdNormal) 
        {
            var parameters = _valueParameters[type];
            return GetValue(parameters.min, parameters.max, parameters.coeff, randStdNormal);
        }
        
        int GetValue(int min, int max, float coeff, float randStdNormal) 
        {                           
            var mean = (max-min)*coeff + min;
            var stdDev = 3; 
            var randNormal = (int)((mean + stdDev * randStdNormal) + 0.5d); 
            
            return Math.Clamp(randNormal, min, max);
        }      
        
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action)
            => _delegates[action];            
    }
}