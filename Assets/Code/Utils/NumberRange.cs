using System;

namespace Utils
{
    public readonly struct NumberRange<T> where T : IComparable<T>
    {
        public readonly T Min;
        public readonly T Max;

        public NumberRange(T min, T max)
        {
            if(max.CompareTo(min) <= 0)
                throw new Exception($"Min value can't be bigger or equal to Max. Values: min = {min} max = {max}");
            Min = min;
            Max = max;
        }
        
        public bool Fits(T valueToCheck)        
			=> Convert.ToBoolean((valueToCheck.CompareTo(Max) - valueToCheck.CompareTo(Min)) >> 31);
    }
}