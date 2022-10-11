using System;
using System.Linq;
using ExtensionMethods;

namespace Utils
{   
    public class ArrayCache<T> : ICache<T>
    {
        int _currentIndex = 0;
        int _size;
        T[] _values;
        
        public ArrayCache(Func<T> generationDelegate, int size)
        {
            _size = size;
            _values = Enumerable.Range(1, _size).Select(entry => generationDelegate()).ToArray();
        }
        
        public ArrayCache(T[] values)
        {
            _values = values;
            _size = values.Count();
        }
        
        public void Shuffle(Random rand)
        {
            rand.Shuffle(_values);
        }
        
        public T Next() 
        {
            var index = ++_currentIndex;
            if(index >= _size)
                _currentIndex = 0;
            return _values[_currentIndex];
        }
    }    
}
