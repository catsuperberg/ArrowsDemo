using GameDesign;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Utils;
using Zenject;
using ExtensionMethods;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{    
    public class OperationFactory
    {          
        int _pairCount = 5;
        int _instanceCount = 5;
        // List<OperationInstance> _instances = new List<OperationInstance>();
        OperationInstance[] _instances;
        int[] _randomIndexes;
        
        IOffsetCache<int> _valueCache;        
        OperationProbabilitiesFactory _probabilities;
        Dictionary<Operation, int> _operationRepeats;
        ICache<float> _floatCache;
        ICache<OperationInstance> _instanceCache;
        ICache<OperationPair> _pairCache;
        
        System.Random _rand;
        IOperationRules _operationRules;
        
        public OperationInstance GetRandom()
            => _instanceCache.Next();
        
        public OperationPair GetRandomPair()
            => _pairCache.Next();
                        
        public OperationPair[] GetInitialSequence(int length)
        {
            var sequence = new OperationPair[length];
            for(int i = 0; i < length; i ++) sequence[i] = new OperationPair(_instanceCache.Next(), _instanceCache.Next());
            return sequence;
        }
                
        public OperationFactory(OperationProbabilitiesFactory probabilities, IOperationRules operationRules)
        {
            _rand = new System.Random(this.GetHashCode());         
            _probabilities = probabilities ?? throw new ArgumentNullException(nameof(probabilities));
            _operationRules = operationRules ?? throw new ArgumentNullException(nameof(operationRules));
        }
        
        public OperationFactory Clone(int numberOfOperations)
            => new OperationFactory(_probabilities, _operationRules, numberOfOperations);
        
        OperationFactory(OperationProbabilitiesFactory probabilities, IOperationRules operationRules, int numberOfOperations)
        {
            _rand = new System.Random(this.GetHashCode());         
            _probabilities = probabilities;
            _operationRules = operationRules;     
            
            // _pairCount = (int)decimal.Round((int)(numberOfOperations*3.5f), 0, MidpointRounding.ToEven);
            // _instanceCount = (int)decimal.Round((int)(_pairCount*2*1.5f), 0, MidpointRounding.ToEven);
            
            _pairCount = 50;
            _instanceCount = 120; 
            
            _operationRepeats = _probabilities.GetRepeatsForCertainCount(_instanceCount);
            _instances = new OperationInstance[_instanceCount];
            
            _randomIndexes = Enumerable.Range(0, _instanceCount).ToArray();
            _rand.Shuffle(_randomIndexes);
            
            Generate();   
        }
        
        void Generate()
        {    
            _floatCache = new ArrayCache<float>(() => (float)_rand.NextDouble(), 31);
            _sqrtValues = new ArrayCache<float>(stdSqrt, 9);
            _sinValues = new ArrayCache<float>(stdSin, 16);
            FillValueCache();
            FillInstances();     
            _pairCache = new ArrayCacheWithEndDelegate<OperationPair>(() => new OperationPair(_instanceCache.Next(), _instanceCache.Next()), _pairCount, ReGenerate);
        }
        
        void ReGenerate()
        { 
            // _floatCache.Shuffle(_rand);   
            // _sqrtValues.Shuffle(_rand);   
            // _sinValues.Shuffle(_rand);   
            _instanceCache.Shuffle(_rand);
            _pairCache = new ArrayCacheWithEndDelegate<OperationPair>(() => new OperationPair(_instanceCache.Next(), _instanceCache.Next()), _pairCount, ReGenerate);
        }      
        
        void FillValueCache()
        {
            var numberOfOffsets = ((int)Operation.Last-1);
            var values = new int[_instanceCount * numberOfOffsets]; // HACK no need for blank values
            FillArrayWithOffset(values, Operation.Multiply, _instanceCount);
            FillArrayWithOffset(values, Operation.Add, _instanceCount);
            FillArrayWithOffset(values, Operation.Subtract, _instanceCount);
            FillArrayWithOffset(values, Operation.Divide, _instanceCount);
            _valueCache = new OffsetArrayCache<int>(values, numberOfOffsets);
        }
        
        void FillArrayWithOffset(int[] array, Operation type, int size)
        {
            var offset = type.ToOffset(size);
            for(int i = offset; i < size+offset; i++) array[i] = RandomValue(type);            
        }
        
        void FillInstances()
        {
            var index = 0;
            var repeatsWithoutBlank = _operationRepeats.Where(kvp => kvp.Key != Operation.Blank);
            foreach(var opearation in repeatsWithoutBlank)
            {
                var type = opearation.Key;
                var mathOperation = _operationRules.GetDelegate(type);
                var reps = opearation.Value;
                
                for(int rep = 0; rep < reps; rep++)
                    // _instances.Add(new OperationInstance(type, _valueCache.Next(type.ToOffset(_pairCount)), mathOperation));
                    _instances[_randomIndexes[index++]] = new OperationInstance(type, _valueCache.Next(type.ToOffset(_instanceCount)), mathOperation); 
            }
            for(int i = 0; i < _operationRepeats[Operation.Blank]; i++) _instances[_randomIndexes[index++]] = OperationInstance.blank;
            _instanceCache = new ArrayCache<OperationInstance>(_instances);        
            // _instanceCache.Shuffle(_rand);    
        }
        
        // void ReFillInstances()
        // {
        //     _instances.ForEach(entry => entry.Update(_valueCache.Next(entry.Type.ToOffset(_pairCount))));
        //     _instanceCache = new ArrayCache<OperationInstance>(_instances.ToArray());        
        //     // _instanceCache.Shuffle(_rand);  
        // }
        
        int RandomValue(Operation type)
        {            
            switch (type)
            {
                case Operation.Multiply: return GetValueWithWeight(_operationRules.GetRange(Operation.Multiply), 0.03f);
                case Operation.Add: return GetValueWithWeight(_operationRules.GetRange(Operation.Add), 0.1f);
                case Operation.Subtract: return GetValueWithWeight(_operationRules.GetRange(Operation.Subtract), 0.5f);
                case Operation.Divide: return GetValueWithWeight(_operationRules.GetRange(Operation.Divide), 0.8f);
                default:
                    return 0;
            }
        }
        
        
        ICache<float> _sqrtValues;
        ICache<float> _sinValues;        
        
        int GetValueWithWeight((int min, int max) range, float coeff) // coeff should be between 0 and 1
        {               
            // coeff = MathUtils.MathClamp(coeff, 0, 1);
            var mean = (range.max-range.min)*coeff + range.min;
            var stdDev = 3; 
            var randStdNormal = _sqrtValues.Next() * _sinValues.Next();
            var randNormal = FastRound(mean + stdDev * randStdNormal); 
            
            return Math.Clamp(randNormal, range.min, range.max);
        }
        
        static int FastRound(float value)
            => (int)(value + 0.5d);
        
        // float stdSqrt()
        //     => System.MathF.Sqrt(stdSqrtArgument());
        float stdSqrt()
            => System.MathF.Pow(stdSqrtArgument(), 0.5f);
        float stdSin()
            => System.MathF.Sin(stdSinArgument());
        float stdSqrtArgument()
            => -2.0f * System.MathF.Log(stdUPoint());
        float stdSinArgument()
            => 2.0f * System.MathF.PI * stdUPoint();
        float stdUPoint()
            => 1.0f-_floatCache.Next();   
        
        
        public class Factory : PlaceholderFactory<OperationFactory>
        {
        }
    }
}
