using System;
using System.Linq;
using ExtensionMethods;

namespace Utils
{   
    public struct ArrayCache<T> : ICache<T> 
    {
        int _currentIndex;
        int _size;
        T[] _values;
        
        public ArrayCache(Func<T> generationDelegate, int size)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _size = size;
            _values = new T[_size];
            for(int i = 0; i < _size; i++) _values[i] = generationDelegate();
        }
        
        public ArrayCache(T[] values)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Count();
        }
        
        public void Update(T[] values)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Count();
        }
        
        public void Shuffle(Random rand)
        {
            rand.Shuffle(_values);
        }
        
        public T Next() 
        {
            return _values[++_currentIndex % _size];
        }
    }    
}
