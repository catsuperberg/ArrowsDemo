using GameMath;
using GameDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationFactory
    {        
        IReadOnlyDictionary<float, Operation> _operationWeights;
        const int _operationCacheSize = 1000;
        const int _floatCacheSize = 1000;
        const int _instanceCacheSize = 1000;
        Operation[] _operationCache;
        int _currentOperationIndex;
        Operation NextOperation() 
        {
            var index = ++_currentOperationIndex;
            if(index >= _operationCacheSize)
                _currentOperationIndex = 0;
            return _operationCache[_currentOperationIndex];
        }
        float[] _floatCache;
        int _currentFloatIndex;
        float NextFloat() 
        {
            var index = ++_currentFloatIndex;
            if(index >= _floatCacheSize)
                _currentFloatIndex = 0;
            return _floatCache[_currentFloatIndex];
        }
        
        
        OperationInstance[] _instanceCache;
        int _currentInstanceIndex;
        OperationInstance NextInstance() 
        {
            var index = ++_currentInstanceIndex;
            if(index >= _floatCacheSize)
                _currentInstanceIndex = 0;
            return _instanceCache[_currentInstanceIndex];
        }
        
        System.Random _rand;
                
        public OperationFactory(MathOperationProbabilities probabilities)
        {
            _operationWeights = probabilities?.OperationsKeyedByWeigh ?? throw new ArgumentNullException(nameof(probabilities)); 
            _rand = new System.Random(this.GetHashCode()); 
            FillOperationCache();     
            FillFloatCache(); 
            FillInstances();
        }
        
        void FillOperationCache()
        {
            _operationCache = Enumerable.Range(1, _operationCacheSize).Select(entry => GetRandomOperation()).ToArray();
        }
        
        void FillFloatCache()
        {
            _floatCache = Enumerable.Range(1, _floatCacheSize).Select(entry => (float)_rand.NextDouble()).ToArray();
        }
        
        void FillInstances()
        {
            _instanceCache = Enumerable.Range(1, _instanceCacheSize).Select(entry => RandomInstance()).ToArray();
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
            return NextInstance();
            // var state = GetRandomState();
            // return new OperationInstance(state.type, state.value);
        }    
        
        OperationInstance RandomInstance()
        {
            var state = GetRandomState();
            return new OperationInstance(state.type, state.value);
        }          
        
        (Operation type, int value) GetRandomState()
        {                        
            var operation = GetOperationWithProbabilityOptimized(0.5f);
            int value = 0;
            switch (operation)
            {
                case Operation.Add:
                    value = GetValueWithWeight(1, 10, 0.1f);
                    break;
                case Operation.Subtract:
                    value = GetValueWithWeight(1, 10, 0.5f);
                    break;
                case Operation.Multiply:
                    value = GetValueWithWeight(2, 4, 0.03f);
                    break;
                case Operation.Divide:
                    value = GetValueWithWeight(2, 5, 0.8f);
                    break;
                case Operation.Blank:
                default:
                    break;
            }
            return (operation, value);
        }
        
        Operation GetOperationWithProbabilityOptimized(float coeff)
        {    
            return NextOperation();
            
            // var randomIndex = _rand.Next(0,_operationCacheSize);
            // return _operationCache[randomIndex];
                     
            // foreach(var operation in _operationWeights)
            //     if(randomWeight < operation.Key)
            //         return operation.Value;
            // throw new System.Exception("GetOperationWithProbabilityOptimized() couldn't find operation suitable for generated randomWeight");
        }
        
        int GetValueWithWeight(float min, float max, float coeff)
        {            
            coeff = MathUtils.MathClamp(coeff, 0, 1);
            var mean = (max-min)*coeff + min;
            var stdDev = 3;
            
            // var u1 = 1.0f-_floatCache.Front(); //uniform(0,1] random doubles
            // var u2 = 1.0f-_floatCache.Front();
            var u1 = 1.0f-NextFloat(); //uniform(0,1] random doubles
            var u2 = 1.0f-NextFloat();
            // var u1 = (float)(1.0-_rand.NextDouble()); //uniform(0,1] random doubles
            // var u2 = (float)(1.0-_rand.NextDouble());
            var randStdNormal = System.MathF.Sqrt(-2.0f * System.MathF.Log(u1)) * System.MathF.Sin(2.0f * System.MathF.PI * u2); //random normal(0,1)
            var randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            randNormal = MathUtils.MathClamp(randNormal, min, max);
            
            return (int)System.MathF.Round(randNormal); 
            
            // var value = (float)(_rand.NextDouble() * max);            
            // return (int)System.MathF.Round(MathUtils.MathClamp(value, min, max));
        }
        
        
        public class Factory : PlaceholderFactory<OperationFactory>
        {
        }
    }
}
