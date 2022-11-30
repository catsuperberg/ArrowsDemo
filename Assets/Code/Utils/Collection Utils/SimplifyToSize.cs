using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class Simplify
    {        
        public static IEnumerable<T> SimplifyToSize<T>(this IEnumerable<T> inputStream, int maxSize)
        {
            var nthToTake = inputStream.Count()/maxSize;
            return inputStream.Where((value, i) => (i + 1) % nthToTake == 0);
        }
    }
}