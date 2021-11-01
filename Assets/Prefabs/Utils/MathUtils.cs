using System;

namespace Utils
{
    public static class MathUtils
    {    
        public static T MathClamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if(val.CompareTo(max) > 0) return max;
            else return val;
        }
        
        public static System.Double RoundToHalf(System.Double passednumber)
        {
            return Math.Round(passednumber * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }    
}