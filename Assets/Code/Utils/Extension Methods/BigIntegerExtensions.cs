using System;
using System.Numerics;

namespace ExtensionMethods
{
    public static class BigIntegerParseToReadable
    {       
        public static int _perPeriod = 3;
             
        public static string ParseToReadable(this BigInteger number)
        {
            var numberString = number.ToString();
            return numberString.Length <= 4 ? numberString : CutToHighestDigits(numberString);
        } 
        
        static string CutToHighestDigits(string number)
        {
            var periodsToStrip = GetPeriodsToStrip(number);
            if(periodsToStrip < 1) 
                throw new Exception("number with less than 1 full digit shouldn't be processed by CutToHighestDigits()");
            
            if(periodsToStrip == 1)
                return ToReadable(number, periodsToStrip, "k");
            else if(periodsToStrip == 2)
                return ToReadable(number, periodsToStrip, "mil");
            else if(periodsToStrip == 3)
                return ToReadable(number, periodsToStrip, "bil");
            else if(periodsToStrip == 4)
                return ToReadable(number, periodsToStrip, "tril");
            else if(periodsToStrip == 5)
                return ToReadable(number, periodsToStrip, "quadr");
            else
                return ToScientific(number);
        }
        
        static int GetPeriodsToStrip(string number)
        {
            var fullPeriods = (int)Math.Floor(number.ToString().Length/(float)_perPeriod);
            return digistBeforeDecimal(number, fullPeriods) <= 0 ? fullPeriods - 1 : fullPeriods;
        }
        
        static string ToReadable(string number, int periodsToStrip, string multiplierString)
        {
            var beforeDecimal = number.Substring(0, digistBeforeDecimal(number, periodsToStrip));
            var afterDecimal = number.Substring(beforeDecimal.Length, 2);
            
            return beforeDecimal + "." + afterDecimal + " " + multiplierString;
        }
        
        static int digistBeforeDecimal(string number, int fullPeriods)
            => number.Length - fullPeriods*_perPeriod;
            
        static string ToScientific(string number)
        {
            var stripped = number.Substring(0, 3).Insert(1, ".");
            return stripped + " E" + (number.Length-1);
        }
    }   
    
    public static class BigIntegerFractionalPower
    {        
        static readonly BigInteger _doubleBigInt = new BigInteger(Double.MaxValue);
        
        public static BigInteger PowFractional(this BigInteger number, double fractionalExponent)
        {
            var intPower = (int)(Math.Floor(fractionalExponent));      
            var fractionalPower = fractionalExponent - intPower;
            
            var intResult = BigInteger.Pow(number, intPower);            
            
            if(number >= _doubleBigInt)
            {
                var multiplier = (int)BigInteger.Divide(number, _doubleBigInt);
                var multiplierPowered = Math.Pow(multiplier, fractionalPower);
                var basePowered = new BigInteger(Math.Pow(Double.MaxValue, fractionalPower));
                var fractionalResultBigInt = basePowered.multiplyByFraction(multiplierPowered);
                return intResult*fractionalResultBigInt;
            }
            else 
            {
                var fractionalResultDouble = Math.Pow((double)number, fractionalPower);
                return intResult.multiplyByFraction(fractionalResultDouble);
            }            
        } 
    }      
}