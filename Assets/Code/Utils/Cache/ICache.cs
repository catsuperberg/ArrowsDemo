using System;

namespace Utils
{
    public interface ICache<T>
    {
        public void Shuffle(Random rand);
        public T Next();
    }
}
