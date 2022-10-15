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
        T[] _values;
        
        public OffsetArrayCache(T[] values, int numberOfOffsets)
        {
            _currentIndex = -1;  // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Count()/numberOfOffsets;
        }
        
        // public void Shuffle(Random rand)
        // {
        //     // rand.Shuffle(_values);
        // }
        
        public T Next(int offset) 
        {
            // _currentIndex = ++_currentIndex % _size;
            // _currentIndex = ++_currentIndex & (_size - 1);
            // var index = ++_currentIndex;
            // if(index >= _size)
            //     _currentIndex = 0;
            // return _values[_currentIndex+offset];
            var index = ++_currentIndex % _size + offset;
            if(offset != 0)
                index = _currentIndex % _size + offset;
            return _values[index];
        }
    }    
}
