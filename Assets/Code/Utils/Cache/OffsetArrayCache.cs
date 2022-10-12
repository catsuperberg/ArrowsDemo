using System;
using System.Linq;
using ExtensionMethods;

namespace Utils
{   
    public struct OffsetArrayCache<T> : IOffsetCache<T> where T : unmanaged
    {
        int _currentIndex;
        int _size;
        T[] _values;
        
        // public OffsetArrayCache(Func<T> generationDelegate, int size, int numberOfOffsets)
        // {
        //     _currentIndex = 0;
        //     _size = size;
        //     _values = Enumerable.Range(1, _size*numberOfOffsets).Select(entry => generationDelegate()).ToArray();
        // }
        
        public OffsetArrayCache(T[] values, int numberOfOffsets)
        {
            _currentIndex = 0;
            _values = values;
            _size = values.Count()/numberOfOffsets;
        }
        
        public void Shuffle(Random rand)
        {
            // rand.Shuffle(_values);
        }
        
        public T Next(int offset) 
        {
            // _currentIndex = ++_currentIndex % _size;
            _currentIndex = ++_currentIndex & (_size - 1);
            // var index = ++_currentIndex;
            // if(index >= _size)
            //     _currentIndex = 0;
            return _values[_currentIndex+offset];
        }
    }    
}
