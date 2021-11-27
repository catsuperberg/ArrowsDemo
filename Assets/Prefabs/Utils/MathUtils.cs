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
            
        public static int RandomSign()
        {
            return (int)((UnityEngine.Random.Range(0,2) - 0.5) * 2);
        } 
        
        public static (int, int) GetPositionOnSpiralGrid(int index)
        {
            // (di, dj) is a vector - direction in which we move right now
            int di = 1;
            int dj = 0;
            // length of current segment
            int segment_length = 1;

            // current position (i, j) and how much of current segment we passed
            int i = 0;
            int j = 0;
            int segment_passed = 0;
            for (int k = 0; k < index; ++k) 
            {
                // make a step, add 'direction' vector (di, dj) to current position (i, j)
                i += di;
                j += dj;
                ++segment_passed;

                if (segment_passed == segment_length) 
                {
                    // done with current segment
                    segment_passed = 0;

                    // 'rotate' directions
                    int buffer = di;
                    di = -dj;
                    dj = buffer;

                    // increase segment length if necessary
                    if (dj == 0) 
                        ++segment_length;
                }
            }
            return (i, j);
        }
    }    
}