using System;

namespace Utils
{
    public interface ICache<T>
    {
        public void Shuffle(Random rand);
        public T[] GetChunkOrRepeated(int size);
        public T Next();
        public T At(int index);
        public void Update(Func<T> generationDelegate);
    }
}
