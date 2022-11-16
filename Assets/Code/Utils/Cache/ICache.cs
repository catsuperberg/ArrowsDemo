using System;
using System.Collections.Generic;

namespace Utils
{
    public interface ICache<T>
    {
        public IReadOnlyCollection<T> Collection {get;}
        public void Shuffle(FastRandom rand);
        public T[] GetChunkOrRepeated(int size);
        public T Next();
        public T At(int index);
        public void Update(Func<T> generationDelegate);        
    }
}
