using System;
using System.Linq;
using ExtensionMethods;

namespace Utils
{   
    public struct ArrayCache<T> : ICache<T> where T : unmanaged
    {
        int _currentIndex;
        int _size;
        T[] _values;
        
        public ArrayCache(Func<T> generationDelegate, int size)
        {
            _currentIndex = 0;
            _size = size;
            _values = Enumerable.Range(1, _size).Select(entry => generationDelegate()).ToArray();
        }
        
        public ArrayCache(T[] values)
        {
            _currentIndex = 0;
            _values = values;
            _size = values.Count();
        }
        
        public void Shuffle(Random rand)
        {
            rand.Shuffle(_values);
        }
        
        public T Next() 
        {
            // _currentIndex = ++_currentIndex % _size;
            _currentIndex = ++_currentIndex & (_size - 1);
            // var index = ++_currentIndex;
            // if(index >= _size)
            //     _currentIndex = 0;
            return _values[_currentIndex];
        }
    }    
}
