using System;
using System.Linq;
using ExtensionMethods;
using Game.Gameplay.Realtime.OperationSequence.Operation;

namespace Utils
{   
    public struct OffsetArrayCache<T> : IOffsetCache<T>
    {
        int _currentIndex;
        int _size;
        int _smallOffset;
        int _bigSize;
        int _bigOffset;
        T[] _values;
        T[] _valuesRepeated;
        
        public OffsetArrayCache(T[] values, int numberOfOffsets, int bigSize)
        {
            _currentIndex = -1;  // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Count();
            _smallOffset = _size/numberOfOffsets;
            _bigSize = bigSize * numberOfOffsets;
            _bigOffset = _bigSize/numberOfOffsets;
            _valuesRepeated = new T[_bigSize];
            RepeatUntilBigSize(numberOfOffsets);
        }
        
        public void RepeatUntilBigSize(int numberOfOffsets)
        {
            for(int offsetTier = 0; offsetTier < numberOfOffsets; offsetTier++)
                for(int i = 0; i < _bigOffset; i++)
                    _valuesRepeated[i+offsetTier*_bigOffset] = _values[(i % _smallOffset) + _smallOffset*offsetTier];
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
        }
        
        public T Next(int offset) 
        {
            return _values[++_currentIndex % _smallOffset + offset];
        }
        
        public T At(int index, int offset)
        {
            return _valuesRepeated[index+offset];
        }
    }    
}
