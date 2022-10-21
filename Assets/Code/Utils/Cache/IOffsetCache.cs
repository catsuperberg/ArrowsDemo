using System;

namespace Utils
{
    public interface IOffsetCache<T>
    {
        // public void Shuffle(Random rand);
        public T Next(int offset);
        public T At(int index, int offset);
    }
}
