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
        int _cacheSize = 90;
        List<OperationInstance> _instances = new List<OperationInstance>();
        
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
        ICache<float> _floatCache;
        ICache<OperationInstance> _instanceCache;
        
        System.Random _rand;
        IOperationDelegates _operationDelegates;
                
        public OperationFactory(OperationProbabilitiesFactory probabilities, IOperationDelegates operationDelegates)
        {
            _rand = new System.Random(this.GetHashCode());
            _operationDelegates = operationDelegates ?? throw new ArgumentNullException(nameof(operationDelegates));              
            _operationRepeats = probabilities?.GetRepeatsForCertainCount(_cacheSize);
            
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
        
        void ReGenerate()
        { 
            _floatCache.Shuffle(_rand);   
            _sqrtValues.Shuffle(_rand);   
            _sinValues.Shuffle(_rand);   
            ReFillInstances();  
        }      
        
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
            foreach(var opearation in _operationRepeats)
            {
                var type = opearation.Key;
                var mathOperation = _operationDelegates.GetDelegate(type);
                var reps = opearation.Value;
                for(int rep = 0; rep < reps; rep++)
                    _instances.Add(new OperationInstance(type, NextValue(type), mathOperation));
            }
            _instanceCache = new ArrayCacheWithEndDelegate<OperationInstance>(_instances.ToArray(), ReGenerate);        
            _instanceCache.Shuffle(_rand);    
        }
        
        void ReFillInstances()
        {
            _instances.ForEach(entry => entry.Update(NextValue(entry.Type)));
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
