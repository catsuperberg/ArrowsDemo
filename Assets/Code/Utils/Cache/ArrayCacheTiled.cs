// using ExtensionMethods;
// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace Utils
// {   
//     public class ArrayCacheTiled<T> : ICache<T> 
//     {
//         public IReadOnlyCollection<T> Collection {get => _values;}
//         int _currentIndex;
//         int _size;
//         int _bigSize;
//         T[] _values;
//         T[] _valuesRepeated;
        
//         public ArrayCacheTiled(Func<T> generationDelegate, int size, int bigSize)
//         {
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//             _size = size;
//             _bigSize = bigSize;
//             _values = new T[size];
//             _valuesRepeated = new T[_bigSize];            
//             var values = _values;
//             for(int i = 0; i < values.Length; i++) values[i] = generationDelegate();
//             _values = values;
//             RepeatUntilBigSize();
//         }
        
//         public ArrayCacheTiled(Func<T> generationDelegate, int size)
//         {
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//             _size = size;
//             _bigSize = _size;
//             _values = new T[_bigSize];            
//             var values = _values;
//             for(int i = 0; i < values.Length; i++) values[i] = generationDelegate();
//             _values = values;
//             _valuesRepeated = _values;
//         }
        
//         public ArrayCacheTiled(T[] values, int bigSize)
//         {
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//             _values = values;
//             _size = values.Count();
//             _bigSize = bigSize;
//             _valuesRepeated = new T[_bigSize];
//             RepeatUntilBigSize();
//         }
        
//         public ArrayCacheTiled(T[] values)
//         {
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//             _values = values;
//             _size = values.Count();
//             _bigSize = _size;
//             _valuesRepeated = _values;
//         }
                
//         public T[] GetChunkOrRepeated(int size)
//         {
//             if(size <= _size)
//             {
//                 var values = _values;
//                 var newValues = new T[size];
//                 for(int i = 0; i < size; i++) newValues[i] = values[i];
//                 return newValues;
//             }
//                 // return _values[0..size];
//                 // return new ArraySegment<T>(_values, 0, size).ToArray();
//             return RepeatUntilSize(size);
//         }
        
//         T[] RepeatUntilSize(int size)
//         {
//             var values = _values;
//             var result = new T[size];
//             for(int i = 0; i < size; i++) result[i] = values[i % values.Length];
//             return result;
//         }
        
        
//         public void Update(Func<T> generationDelegate)
//         {
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//             _values = new T[_size];
//             for(int i = 0; i < _size; i++) _values[i] = generationDelegate();
//         }
        
//         public void Update(T[] values, int bigSize)
//         {
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//             _values = values;
//             _size = values.Count();
//             _bigSize = bigSize;
//             RepeatUntilBigSize();
//         }
        
//         public void RepeatUntilBigSize()
//         {
//             for(int i = 0; i < _bigSize; i++) _valuesRepeated[i] = Next();
//             _currentIndex = -1; // HACK so that Next counts form 0 and not 1
//         }
        
//         public void Shuffle(Random rand)
//         {
//             rand.Shuffle(_values);
//         }
        
//         public T Next() 
//         {
//             return _values[++_currentIndex % _size];
//         }
        
//         public T At(int index) 
//         {            
//             return _valuesRepeated[index];
//         }
//     }    
// }
