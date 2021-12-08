using PeterO.Numbers;
using System;
using System.Numerics;

namespace Utils
{
    public class HalfLifeCalculator
    {
        EDecimal _halfLife;        
        
        public HalfLifeCalculator(BigInteger startValue, BigInteger endValue, double timeToReachEnd)
        {            
            EInteger count = EInteger.FromString(startValue.ToString());
            EInteger finalCount = EInteger.FromString(endValue.ToString());
            var finalCountPart = EDecimal.FromEInteger(finalCount).Divide(EDecimal.FromEInteger(count), EContext.Binary64);            
            var k = finalCountPart.Log(EContext.Binary64).Divide(EDecimal.FromDouble(timeToReachEnd), EContext.Binary64);
            _halfLife = EDecimal.FromDouble(Math.Log(0.5)).Divide(k, EContext.Binary64);
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