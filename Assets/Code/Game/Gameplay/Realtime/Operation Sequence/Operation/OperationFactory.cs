using System.Collections.Generic;
using System.Linq;
using Utils;
using Zenject;
using ExtensionMethods;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{    
    public class OperationFactory
    {          
        const int _instanceCount = 80; 
        const int _pairCount = _instanceCount/2;
        static IReadOnlyDictionary<Operation, int> _operationRepeats;
              
        ICache<float> _stdNormals;
        IOffsetCache<int> _valueCache;  
        
        int[] _randomIndexes;
        ICache<OperationInstance> _instanceCache;
        ICache<OperationPair> _pairCache;
        ICache<OperationPair> _positivePairs;
        
        FastRandom _rand;
        public readonly IOperationRules OperationRules;
                
        public OperationPair GetRandomPair()
            => _pairCache.Next();
            
        public OperationPair GetPositivePair()
            => _positivePairs.Next();
                        
        public OperationPair[] GetInitialSequence(int length)
        {            
            var result = _pairCache.GetChunkOrRepeated(length);
            _pairCache.Shuffle(_rand);
            return result;
        }    
                
        public OperationFactory(IOperationRules operationRules)
        {
            _rand = new FastRandom(this.GetHashCode());  
            OperationRules = operationRules;     
            
            if(_operationRepeats == null)
                _operationRepeats = operationRules.OperationRepeats(_instanceCount);
            
            _randomIndexes = Enumerable.Range(0, _instanceCount).ToArray();
            _randomIndexes.Shuffle(_rand);
            
            Generate();   
            var positivePairs = _pairCache.Collection.Where(entry => entry.LeftOperation.Type.IsPositive() || entry.RightOperation.Type.IsPositive());
            _positivePairs = new ArrayCache<OperationPair>(positivePairs.ToArray());
        }
        
        void Generate()
        {    
            _stdNormals = new ArrayCache<float>(() => {return stdSqrt()*stdSin();}, _instanceCount);
            FillValueCache();
            FillInstances();     
            var instance = 0;
            _pairCache = new ArrayCacheWithEndDelegate<OperationPair>(() => new OperationPair(_instanceCache.At(instance++), _instanceCache.At(instance++), OperationRules), _pairCount, ReGenerate);
        }  
        
        float stdSqrt() => System.MathF.Sqrt(-2.0f * System.MathF.Log(stdUPoint));
        float stdSin() => System.MathF.Sin(2.0f * System.MathF.PI * stdUPoint);
        float stdUPoint => 1.0f-(float)_rand.NextDouble(); 
        
        void FillValueCache()
        {
            var smallSize = _instanceCount/4;
            var numberOfOffsets = ((int)Operation.Last-1); // HACK no need for blank values
            var values = new int[smallSize * numberOfOffsets]; 
            FillArrayWithOffset(values, Operation.Multiply, smallSize);
            FillArrayWithOffset(values, Operation.Add, smallSize);
            FillArrayWithOffset(values, Operation.Subtract, smallSize);
            FillArrayWithOffset(values, Operation.Divide, smallSize);
            _valueCache = new OffsetArrayCache<int>(values, numberOfOffsets, _instanceCount);
        }
        
        void FillArrayWithOffset(int[] array, Operation type, int size)
        {
            var offset = type.ToOffset(size);
            for(int i = offset; i < size+offset; i++) array[i] = OperationRules.GetValueForType(type, _stdNormals.At(i-offset));           
        }
        
        
        void FillInstances()
        {
            var _instances = new OperationInstance[_instanceCount];
            var index = 0;
            var repeatsWithoutBlank = _operationRepeats.Where(kvp => kvp.Key != Operation.Blank);
            foreach(var opearation in repeatsWithoutBlank)
            {
                var type = opearation.Key;
                var mathOperation = OperationRules.GetDelegate(type);
                var reps = opearation.Value;
                
                for(int rep = 0; rep < reps; rep++)
                    _instances[_randomIndexes[index++]] = new OperationInstance(type, _valueCache.At(index, type.ToOffset(_instanceCount)), mathOperation); 
            }
            for(int i = 0; i < _operationRepeats[Operation.Blank]; i++) _instances[_randomIndexes[index++]] = OperationInstance.blank;
            _instanceCache = new ArrayCache<OperationInstance>(_instances);   
        }        
        
        void ReGenerate()
        { 
            _instanceCache.Shuffle(_rand);
            var instance = 0;
            _pairCache.Update(() => new OperationPair(_instanceCache.At(instance++), _instanceCache.At(instance++), OperationRules));
        }      
        
        
        public class Factory : PlaceholderFactory<OperationFactory>
        {
        }
    }
}
