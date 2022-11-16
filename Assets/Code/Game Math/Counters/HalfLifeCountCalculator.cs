using System;
using System.Numerics;
using ExtensionMethods;

namespace GameMath
{
    public class HalfLifeCountCalculator
    {
        double _halfLife;  
        static double _logOfHalf = Math.Log(0.5);
        
        public HalfLifeCountCalculator(BigInteger startValue, BigInteger endValue, double timeToReachEnd)
        {            
            ExceptionOnInvalidEndValue(endValue);
            ExceptionOnCountUp(startValue, endValue);
            var logOfFinalCountPart = BigInteger.Log(endValue) - BigInteger.Log(startValue);
            var k = logOfFinalCountPart/timeToReachEnd;
            _halfLife = _logOfHalf/k;
        }
        
        void ExceptionOnInvalidEndValue(BigInteger endValue)
        {
            if(endValue < 1)
                throw new Exception("Half life count calculator can only process decay end value of 1 and up");
        }
        
        void ExceptionOnCountUp(BigInteger startValue, BigInteger endValue)
        {            
            if(startValue <= endValue)
                throw new Exception("Half life count calculator can only process values going down");
        }
        
        public BigInteger CalculateDecayed(BigInteger startValue, double timePassed)
        {
            var timePower = timePassed/_halfLife;
            var frameCoeff = Math.Pow(0.5, timePower);
            // return startValue - (startValue.multiplyByFraction(frameCoeff));
            return startValue - (startValue.multiplyByFractionFast(frameCoeff));
        }
    }
}