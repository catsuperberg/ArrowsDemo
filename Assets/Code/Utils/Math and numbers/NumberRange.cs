using System;

namespace Utils
{
    public static class NumberFormater
    {
        public static float RoundSmallValue(float value, int digitsAfterLastZero = 2)
        {
            var leadingZeroes = (int)(-1 - Math.Floor(Math.Log10(value % 1)));
            return (float)Math.Round(value, leadingZeroes+digitsAfterLastZero);
        }
    }
}