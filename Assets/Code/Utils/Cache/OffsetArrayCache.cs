namespace Utils
{   
    public class OffsetArrayCache<T> : IOffsetCache<T>
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
            _size = values.Length;
            _smallOffset = _size/numberOfOffsets;
            _bigSize = bigSize * numberOfOffsets;
            _bigOffset = _bigSize/numberOfOffsets;
            RepeatUntilBigSize(numberOfOffsets);
        }
        
        public void RepeatUntilBigSize(int numberOfOffsets)
        {
            // var values = _values;
            _valuesRepeated = new T[_bigSize];
            for(int offsetTier = 0; offsetTier < numberOfOffsets; offsetTier++)
                for(int i = 0; i < _bigOffset; i++)
                    _valuesRepeated[i+offsetTier*_bigOffset] = _values[(i % _smallOffset) + _smallOffset*offsetTier];
            // _valuesRepeated = valuesRepeatred;       
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
        }
        
        public T Next(int offset) 
            => _values[++_currentIndex % _smallOffset + offset];
        
        public T At(int index, int offset)
            => _valuesRepeated[index+offset];
    }    
}
