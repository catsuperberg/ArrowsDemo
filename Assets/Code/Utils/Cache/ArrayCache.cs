using System;

namespace Utils
{   
    public class ArrayCache<T> : BaseArrayCache<T> 
    {
        public ArrayCache(Func<T> generationDelegate, int size)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _size = size;
            _values = new T[size];          
            var values = _values;
            for(int i = 0; i < values.Length; i++) values[i] = generationDelegate();                        
            _values = values;
            GenerateShuffledIndexes();
        }
        
        public ArrayCache(T[] values)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Length;
            GenerateShuffledIndexes();
        }
    }    
}
