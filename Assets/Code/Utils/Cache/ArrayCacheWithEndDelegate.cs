using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{   
    public class ArrayCacheWithEndDelegate<T> : ICache<T>
    {
        public IReadOnlyCollection<T> Collection {get => _values;}
        int _currentIndex;
        int _size;
        T[] _values;
        
        Action _endReachedDelegate;
        
        public ArrayCacheWithEndDelegate(Func<T> generationDelegate, int size, Action endReachedDelegate)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _size = size;
            _values = new T[_size];
            for(int i = 0; i < _size; i++) _values[i] = generationDelegate();
            _endReachedDelegate = endReachedDelegate;
        }
        
        public ArrayCacheWithEndDelegate(T[] values, Action endReachedDelegate)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1
            _values = values;
            _size = values.Count();
            _endReachedDelegate = endReachedDelegate;
        }        
        
        public void Update(Func<T> generationDelegate)
        {
            for(int i = 0; i < _size; i++) _values[i] = generationDelegate();
        }
        
        public void Shuffle(Random rand)
        {
            rand.Shuffle(_values);
        }
        
        public T[] GetChunkOrRepeated(int size)
        {
            if(size <= _size)
                return new ArraySegment<T>(_values, 0, size).ToArray();
            return RepeatUntilSize(size);
        }
        
        T[] RepeatUntilSize(int size)
        {
            var result = new T[size];
            for(int i = 0; i < size; i++) result[i] = _values[i % _size];
            return result;
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
        
        public T At(int index) 
        {
            return _values[index % _size];
        }
    }    
}
