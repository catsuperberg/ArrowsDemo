using System;
using System.Collections.Generic;

namespace Utils
{
    public static class EnumUtils
    {         
        public static bool InRange(int value, int first, int last)
            => (value >= first && value <= last);
    }
}