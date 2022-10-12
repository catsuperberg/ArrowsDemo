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
        int _cacheSize = 64;  // must be base 2
        List<OperationInstance> _instances = new List<OperationInstance>();
        
        IOffsetCache<int> _valueCache;
        // Dictionary<Operation, int[]> _valueCaches;
        // int _currentValueIndex;
        // int NextValue(Operation type) 
        // {
        //     var index = ++_currentValueIndex;
        //     if(index >= _cacheSize)
        //         _currentValueIndex = 0;
        //     return _valueCaches[type][_currentValueIndex];
        // }
        
        Dictionary<Operation, int> _operationRepeats;
        ICache<float> _floatCache;
        ICache<OperationInstance> _instanceCache;
        
        System.Random _rand;
        IOperationDelegates _operationDelegates;
                
        public OperationFactory(OperationProbabilitiesFactory probabilities, IOperationDelegates operationDelegates)
        {
            _rand = new System.Random(this.GetHashCode());         
            _operationRepeats = probabilities?.GetRepeatsForCertainCount(_cacheSize);
            _operationDelegates = operationDelegates ?? throw new ArgumentNullException(nameof(operationDelegates));     
            
            // Generate();   
        }
        
        OperationFactory(Dictionary<Operation, int> repeats, IOperationDelegates operationDelegates)
        {
            _rand = new System.Random(this.GetHashCode());         
            _operationRepeats = repeats;
            _operationDelegates = operationDelegates;     
            
            Generate();   
        }
        
        void Generate()
        {    
            _floatCache = new ArrayCache<float>(() => (float)_rand.NextDouble(), _cacheSize);
            _sqrtValues = new ArrayCache<float>(stdSqrt, _cacheSize);
            _sinValues = new ArrayCache<float>(stdSin, _cacheSize);
            FillValueCache();
            FillInstances();     
        }
        
        public OperationFactory Clone()
            => new OperationFactory(_operationRepeats, _operationDelegates);
        
        void ReGenerate()
        { 
            _floatCache.Shuffle(_rand);   
            _sqrtValues.Shuffle(_rand);   
            _sinValues.Shuffle(_rand);   
            ReFillInstances();  
        }      
        
        void FillValueCache()
        {
            var values = new int[_cacheSize * (int)Operation.Last];
            FillArrayWithOffset(values, Operation.Add, _cacheSize);
            FillArrayWithOffset(values, Operation.Subtract, _cacheSize);
            FillArrayWithOffset(values, Operation.Multiply, _cacheSize);
            FillArrayWithOffset(values, Operation.Divide, _cacheSize);
            FillArrayWithOffset(values, Operation.Blank, _cacheSize);
            
            
            // values.AddRange(Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Add)));
            // values.AddRange(Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Subtract)));
            // values.AddRange(Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Multiply)));
            // values.AddRange(Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Divide)));
            // values.AddRange(Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Blank)));
            
            _valueCache = new OffsetArrayCache<int>(values, _cacheSize);
            // _valueCaches = new Dictionary<Operation, int[]>();
            // _valueCaches.Add(Operation.Add, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Add)).ToArray());
            // _valueCaches.Add(Operation.Subtract, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Subtract)).ToArray());
            // _valueCaches.Add(Operation.Multiply, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Multiply)).ToArray());
            // _valueCaches.Add(Operation.Divide, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Divide)).ToArray());
            // _valueCaches.Add(Operation.Blank, Enumerable.Range(1, _cacheSize).Select(entry => RandomValue(Operation.Blank)).ToArray());
        }
        
        void FillArrayWithOffset(int[] array, Operation type, int size)
        {
            var offset = type.ToOffset(size);
            for(int i = offset; i < _cacheSize+offset; i++) array[i] = RandomValue(type);            
        }
        
        void FillInstances()
        {
            foreach(var opearation in _operationRepeats)
            {
                var type = opearation.Key;
                var mathOperation = _operationDelegates.GetDelegate(type);
                var reps = opearation.Value;
                // var operationPrototype = new OperationInstance(type, 1, mathOperation);
                // for(int rep = 0; rep < reps; rep++)
                //     _instances.Add(operationPrototype.Clone());
                // _instances.AddRange(Enumerable.Repeat(operationPrototype, reps));
                // for(int rep = 0; rep < reps; rep++)
                // {
                //     var newOperation = operationPrototype;
                //     _instances.Add(newOperation);
                // }
                
                for(int rep = 0; rep < reps; rep++)
                    _instances.Add(new OperationInstance(type, _valueCache.Next(type.ToOffset(_cacheSize)), mathOperation));
            }
            // _instances.ForEach(entry => entry.Update(NextValue(entry.Type)));  // Doesn't work for some reason
            _instanceCache = new ArrayCacheWithEndDelegate<OperationInstance>(_instances.ToArray(), ReGenerate);        
            _instanceCache.Shuffle(_rand);    
        }
        
        void ReFillInstances()
        {
            _instances.ForEach(entry => entry.Update(_valueCache.Next(entry.Type.ToOffset(_cacheSize))));
            _instanceCache = new ArrayCacheWithEndDelegate<OperationInstance>(_instances.ToArray(), ReGenerate);        
            _instanceCache.Shuffle(_rand);  
        }
        
        public OperationInstance GetRandom()
        {
            return _instanceCache.Next();
        }    
        
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
        
        
        ICache<float> _sqrtValues;
        ICache<float> _sinValues;        
        
        int GetValueWithWeight(float min, float max, float coeff)
        {               
            coeff = MathUtils.MathClamp(coeff, 0, 1);
            var mean = (max-min)*coeff + min;
            var stdDev = 3; 
            var randStdNormal = _sqrtValues.Next() * _sinValues.Next();
            var randNormal = mean + stdDev * randStdNormal; 
            randNormal = MathUtils.MathClamp(randNormal, min, max);
            
            return (int)System.MathF.Round(randNormal);             
        }
        
        
        
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
