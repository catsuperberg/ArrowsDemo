using GameDesign;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Utils;
using Zenject;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{    
    public class OperationFactory
    {          
        IReadOnlyDictionary<float, Operation> _operationWeights;
        int _cacheSize = 60;
        
        Dictionary<Operation, int[]> _valueCaches;
        int _currentValueIndex;
        int NextValue(Operation type) 
        {
            var index = ++_currentValueIndex;
            if(index >= _cacheSize)
                _currentValueIndex = 0;
            return _valueCaches[type][_currentValueIndex];
        }
        
        Dictionary<Operation, int> _operationRepeats;
        // ICache<Operation> _operationCache;
        ICache<float> _floatCache;
        ICache<OperationInstance> _instanceCache;
        
        System.Random _rand;
        IOperationDelegates _operationDelegates;
                
        public OperationFactory(OperationProbabilitiesFactory probabilities, IOperationDelegates operationDelegates)
        {
            _operationWeights = probabilities?.GetFromGeneratedJson().OperationsKeyedByWeigh ?? throw new ArgumentNullException(nameof(probabilities)); 
            _rand = new System.Random(this.GetHashCode());
            _operationDelegates = operationDelegates ?? throw new ArgumentNullException(nameof(operationDelegates));   
            
            _operationRepeats = probabilities.GetRepeatsForCertainCount(_cacheSize);
            
            Generate();   
        }
        
        void Generate()
        {    
            // FillOperationRepeats();
            // _operationCache = new ArrayCache<Operation>(GetRandomOperation, _cacheSize);
            _floatCache = new ArrayCache<float>(() => (float)_rand.NextDouble(), _cacheSize);
            _sqrtValues = new ArrayCache<float>(stdSqrt, _cacheSize);
            _sinValues = new ArrayCache<float>(stdSin, _cacheSize);
            FillValueCache();
            FillInstances();     
        }
        
        void ReGenerate()
        { 
            // _operationCache.Shuffle(_rand);  
            _floatCache.Shuffle(_rand);   
            _sqrtValues.Shuffle(_rand);   
            _sinValues.Shuffle(_rand);   
            FillInstances();   
        }      
        
        // void FillOperationRepeats()
        // {
        //     var sortedKeys = from entry in _operationWeights orderby entry.Key descending select entry.Key;
        //     var smallestKey = sortedKeys.Last();
        //     var toSingleSmallestCoeff = 1f/smallestKey;
        //     var intWeights = _operationWeights.ToDictionary(entry => entry.Value, entry => (int)MathF.Round(entry.Key * toSingleSmallestCoeff)); // flipped dictionary 
        //     var coeffToCacheSize = _cacheSize/(float)(intWeights.Sum(entry => entry.Value));
        //     if(coeffToCacheSize <= 1)   
        //     {
        //         _cacheSize = (int)(intWeights.Sum(entry => entry.Value));
        //         coeffToCacheSize = 1;
        //     }
        //     var intRepeats = intWeights.ToDictionary(entry => entry.Key, entry => (int)MathF.Round(entry.Value * coeffToCacheSize));
        //     var repeatsDeficit = _cacheSize - intWeights.Values.Sum();
            
        //     var sortedRepeats = from entry in intRepeats orderby entry.Value descending select entry;
        //     var largestEntry = sortedRepeats.First();
        //     var newLargestKey = largestEntry.Value + repeatsDeficit;
            
        //     var finalRepeats = sortedRepeats.ToDictionary(entry => entry.Key, entry => entry.Value);
        //     if(!sortedRepeats.Any(entry => entry.Value == (largestEntry.Value)))
        //     {
        //         finalRepeats.Remove(largestEntry.Key);
        //         finalRepeats.Add(largestEntry.Key, newLargestKey);
        //     }
            
            
        //     _operationRepeats = finalRepeats;
        // }
        
        void FillValueCache()
        {
            _valueCaches = new Dictionary<Operation, int[]>();
            _valueCaches.Add(Operation.Add, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Add)).ToArray());
            _valueCaches.Add(Operation.Subtract, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Subtract)).ToArray());
            _valueCaches.Add(Operation.Multiply, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Multiply)).ToArray());
            _valueCaches.Add(Operation.Divide, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Divide)).ToArray());
            _valueCaches.Add(Operation.Blank, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Blank)).ToArray());
        }
        
        void FillInstances()
        {
            var instances = new List<OperationInstance>();
            foreach(var opearation in _operationRepeats)
            {
                var type = opearation.Key;
                var mathOperation = _operationDelegates.GetDelegate(type);
                var reps = opearation.Value;
                foreach(var rep in Enumerable.Range(0, reps))
                    instances.Add(new OperationInstance(type, NextValue(type), mathOperation));
            }
            _instanceCache = new ArrayCacheWithEndDelegate<OperationInstance>(instances.ToArray(), ReGenerate);        
            _instanceCache.Shuffle(_rand);    
            // _instanceCache = new ArrayCacheWithEndDelegate<OperationInstance>(RandomInstance, _cacheSize, ReGenerate);
        }
        
        Operation GetRandomOperation()
        {              
            var randomWeight = _rand.NextDouble();                      
            foreach(var operation in _operationWeights)
                if(randomWeight < operation.Key)
                    return operation.Value;
            throw new System.Exception("GetOperationWithProbabilityOptimized() couldn't find operation suitable for generated randomWeight");
        }
        
        public OperationInstance GetRandom()
        {
            return _instanceCache.Next();
            // var state = GetRandomState();
            // return new OperationInstance(state.type, state.value);
        }    
        
        // OperationInstance RandomInstance()
        // {
        //     // var state = GetRandomState();
        //     // return new OperationInstance(state.type, state.value, _operationDelegates);
        //     var operation = _operationCache.Next();
        //     return new OperationInstance(operation, NextValue(operation), _operationDelegates);
        // }          
        
        // (Operation type, int value) GetRandomState()
        // {                        
        //     var operation = GetOperationWithProbabilityOptimized(0.5f);
        //     int value = RandomValue(operation);
        //     return (operation, value);
        // }
        
        int RandomValue(Operation type)
        {            
            switch (type)
            {
                case Operation.Add: return GetValueWithWeight(1, 10, 0.1f);
                case Operation.Subtract: return GetValueWithWeight(1, 10, 0.5f);
                case Operation.Multiply: return GetValueWithWeight(2, 4, 0.03f);
                case Operation.Divide: return GetValueWithWeight(2, 5, 0.8f);
                case Operation.Blank:
                default:
                    return 0;
            }
        }
        
        // Operation GetOperationWithProbabilityOptimized(float coeff)
        // {    
        //     return _operationCache.Next();
            
        //     // var randomIndex = _rand.Next(0,_operationCacheSize);
        //     // return _operationCache[randomIndex];
                     
        //     // foreach(var operation in _operationWeights)
        //     //     if(randomWeight < operation.Key)
        //     //         return operation.Value;
        //     // throw new System.Exception("GetOperationWithProbabilityOptimized() couldn't find operation suitable for generated randomWeight");
        // }
        
        
        ICache<float> _sqrtValues;
        ICache<float> _sinValues;        
        
        int GetValueWithWeight(float min, float max, float coeff)
        {               
            coeff = MathUtils.MathClamp(coeff, 0, 1);
            var mean = (max-min)*coeff + min;
            var stdDev = 3;
            
            // var stdSqrtArgument = -2.0f * System.MathF.Log(stdUPoint());
            // var stdSinArgument = 2.0f * System.MathF.PI * stdUPoint();
            // var randStdNormal = System.MathF.Sqrt(stdSqrtArgument) * System.MathF.Sin(stdSinArgument); 
            var randStdNormal = _sqrtValues.Next() * _sinValues.Next();
            var randNormal = mean + stdDev * randStdNormal; 
            randNormal = MathUtils.MathClamp(randNormal, min, max);
            
            return (int)System.MathF.Round(randNormal);             
        }
        
        
        
        float stdSqrt()
            => System.MathF.Sqrt(stdSqrtArgument());
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
