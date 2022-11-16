using System;

namespace Utils
{   
    public class ArrayCacheWithEndDelegate<T> : BaseArrayCache<T>
    {        
        Action _endReachedDelegate;
        
        public ArrayCacheWithEndDelegate(Func<T> generationDelegate, int size, Action endReachedDelegate)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _size = size;
            _values = new T[_size];
            var values = _values;
            for(int i = 0; i < values.Length; i++) values[i] = generationDelegate();
            _values = values;
            _endReachedDelegate = endReachedDelegate;
            GenerateShuffledIndexes();
        }
        
        public ArrayCacheWithEndDelegate(T[] values, Action endReachedDelegate)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Length;            
            _endReachedDelegate = endReachedDelegate;
            GenerateShuffledIndexes();
        }        
        
        public override T Next() 
        {
            var index = ++_currentIndex;
            if(index >= _size)
            {
                _currentIndex = 0;
                _endReachedDelegate();
            }
            return _values[_shuffledIndexes[_currentIndex]];
        }
    }    
}
