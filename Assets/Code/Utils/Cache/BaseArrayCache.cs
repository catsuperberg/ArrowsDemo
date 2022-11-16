using ExtensionMethods;
using System;
using System.Collections.Generic;

namespace Utils
{   
    public abstract class BaseArrayCache<T> : ICache<T> 
    {
        static Random _rand = new Random();
        public IReadOnlyCollection<T> Collection {get => _values;}
        protected int _currentIndex;
        protected int _size;
        protected T[] _values;
        protected int[] _shuffledIndexes;
        
        protected void GenerateShuffledIndexes()
        {            
            var indexes = new int[_values.Length];
            for(int i = 0; i < indexes.Length; i++) indexes[i] = i;
            indexes.Shuffle(_rand);
            _shuffledIndexes = indexes;
        }
            
        public T[] GetChunkOrRepeated(int size)
        {
            var result = new T[size];
            for(int i = 0; i < result.Length; i++) result[i] = _values[_shuffledIndexes[i % _shuffledIndexes.Length]];
            return result;
            
            // var result = new T[size];
            // var values = new T[_size];
            // for(int i = 0; i < values.Length; i++) values[i] = _values[_shuffledIndexes[i]];
            // // if(size <= values.Length)
            // // {
            // //     Array.Copy(values, result, size);
            // //     return result;
            // // }
            
            // var fullCopieCount = size/values.Length;
            // var partialCopyCount = size-(fullCopieCount*values.Length);
            // for(int i = 0; i < fullCopieCount; i++)
            //     Array.Copy(values, 0, result, i*values.Length, values.Length);
            // Array.Copy(values, 0, result, fullCopieCount*values.Length, partialCopyCount);
            // return result;
        }
                
        // public T[] GetChunkOrRepeated(int size)
        // {
        //     // if(size <= _size)
        //     // {
        //     //     var values = _values;
        //     //     var newValues = new T[size];
        //     //     for(int i = 0; i < size; i++) newValues[i] = values[_shuffledIndexes[i]];
        //     //     return newValues;
        //     // }
        //     //     // return _values[0..size];
        //     //     // return new ArraySegment<T>(_values, 0, size).ToArray();
        //     return RepeatUntilSize(size);
        // }
        
        // T[] RepeatUntilSize(int size)
        // {
        //     var indexes = _shuffledIndexes;
        //     var values = _values;
        //     var result = new T[size];
        //     for(int i = 0; i < size; i++) result[i] = values[indexes[i % values.Length]];
        //     return result;
        // }        
        
        public virtual void Update(Func<T> generationDelegate)
        {
            _currentIndex = -1; // HACK so that Next counts form 0 and not 1            
            var values = _values;
            for(int i = 0; i < values.Length; i++) values[i] = generationDelegate();
            _values = values;
        }
        
        public void Shuffle(FastRandom rand)
        {
            // rand.Shuffle(_shuffledIndexes);
            _shuffledIndexes.Shuffle(rand);
        }
        
        public virtual T Next() 
            => _values[_shuffledIndexes[++_currentIndex % _size]];
        
        public T At(int index) 
            => _values[_shuffledIndexes[index]];
    }    
}
