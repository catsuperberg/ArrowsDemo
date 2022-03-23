using System;
using System.Numerics;

namespace ExtensionMethods
{
    public static class BigIntegerParseToReadable
    {            
        const int period = 3;
        
        public static string ParseToReadable(this BigInteger number)
        {
            var digitsInNumber = CountDigitsPositive(number);
            if(IsInNthPeriod(digitsInNumber, 1))
                return number.ToString();
            else if(IsInNthPeriod(digitsInNumber, 2))
                return ToReadable(number, digitsInNumber, "k");
            else if(IsInNthPeriod(digitsInNumber, 3))
                return ToReadable(number, digitsInNumber, "mil");
            else if(IsInNthPeriod(digitsInNumber, 4))
                return ToReadable(number, digitsInNumber, "bil");
            else if(IsInNthPeriod(digitsInNumber, 5))
                return ToReadable(number, digitsInNumber, "tril");
            else if(IsInNthPeriod(digitsInNumber, 6))
                return ToReadable(number, digitsInNumber, "quadr");
            else
                return ToReadable(number, digitsInNumber, "E" + (digitsInNumber/period)*period);
        } 
        
        static bool IsInNthPeriod(int digits, int NthPeriod)
        {
            var digitsInNthPeriod = NthPeriod * period;
            return digits < (digitsInNthPeriod + 1);
        }
        
        static int CountDigitsPositive(BigInteger number) 
        {
            return (int)Math.Floor(BigInteger.Log10(number) + 1);
        }
        
        static string ToReadable(BigInteger number, int digits, string multiplierString)
        {
            var fullString = number.ToString();
            var fullPeriods = (digits-1)/period;
            var digitsInHighestPeriod = digits - (fullPeriods*period);
            var beforeDecimal = fullString.Substring(0, digitsInHighestPeriod);
            var afterDecimal = fullString.Substring(digitsInHighestPeriod, 2);
            
            return beforeDecimal + "." + afterDecimal + " " + multiplierString;
        }
    }       
}