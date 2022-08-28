using PeterO.Numbers;
using System;
using System.Numerics;

namespace GameMath
{
    public class HalfLifeCountCalculator
    {
        EDecimal _halfLife;        
        
        public HalfLifeCountCalculator(BigInteger startValue, BigInteger endValue, double timeToReachEnd)
        {            
            ExceptionOnInvalidEndValue(endValue);
            ExceptionOnCountUp(startValue, endValue);
            
            var count = EInteger.FromString(startValue.ToString());
            var finalCount = EInteger.FromString(endValue.ToString());
            var finalCountPart = EDecimal.FromEInteger(finalCount).Divide(EDecimal.FromEInteger(count), EContext.Binary64);            
            var k = finalCountPart.Log(EContext.Binary64).Divide(EDecimal.FromDouble(timeToReachEnd), EContext.Binary64);
            _halfLife = EDecimal.FromDouble(Math.Log(0.5)).Divide(k, EContext.Binary64);
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
            var count = EInteger.FromString(startValue.ToString());
            var frameCoeff = ((EDecimal.FromDouble(timePassed).Divide(_halfLife, EContext.Binary64)) * 
                    (EDecimal.FromDouble(Math.Log(0.5)))).Exp(EContext.Binary64);
            var frameDamage = count - (count * frameCoeff).ToEInteger();
            return BigInteger.Parse(frameDamage.ToString());
        }
    }
}