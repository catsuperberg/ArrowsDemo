using System;
using Utils;
using System.Linq;

namespace ExtensionMethods
{    
    public static class ArrayShuffle
    {
        public static void Shuffle<T>(this T[] array, FastRandom rand)
        {
            // array.OrderBy(value => rand.Next(array.Length));
            int n = array.Length;
            while (n > 1) 
            {
                int k = rand.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}