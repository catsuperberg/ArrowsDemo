using System;
using System.Linq;
using ExtensionMethods;

namespace Utils
{   
    public class ArrayCacheWithEndDelegate<T> : ICache<T>
    {
        int _currentIndex = 0;
        int _size;
        T[] _values;
        
        Action _endReachedDelegate;
        
        public ArrayCacheWithEndDelegate(Func<T> generationDelegate, int size, Action endReachedDelegate)
        {
            _size = size;
            _values = Enumerable.Range(1, _size).Select(entry => generationDelegate()).ToArray();
            _endReachedDelegate = endReachedDelegate;
        }
        
        public ArrayCacheWithEndDelegate(T[] values, Action endReachedDelegate)
        {
            _values = values;
            _size = values.Count();
            _endReachedDelegate = endReachedDelegate;
        }
        
        public void Shuffle(Random rand)
        {
            rand.Shuffle(_values);
        }
        
        public T Next() 
        {
            var index = ++_currentIndex;
            if(index >= _size)
            {
                _currentIndex = 0;
                _endReachedDelegate();
            }
            return _values[_currentIndex];
        }
    }    
}
