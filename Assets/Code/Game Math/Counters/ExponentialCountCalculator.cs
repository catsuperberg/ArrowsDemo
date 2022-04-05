using PeterO.Numbers;
using System;
using System.Numerics;

namespace GameMath
{
    public class ExponentialCountCalculator
    {          
        public readonly BigInteger StartValue;
        public readonly BigInteger EndValue;
        public readonly double PeriodFromStartToEnd;
        
        HalfLifeCountCalculator _halfLifeCount = null;        
        
        public ExponentialCountCalculator(BigInteger startValue, BigInteger endValue, double periodFromStartToEnd)
        {            
            StartValue = startValue;
            EndValue = endValue;
            PeriodFromStartToEnd = periodFromStartToEnd;
            
            CreateHalfLifeCount();            
        }
        
        void CreateHalfLifeCount()
        {
            var largerValue = (StartValue >= EndValue) ? StartValue : EndValue;
            var smallerValue = (StartValue >= EndValue) ? EndValue : StartValue;
            if(smallerValue < 1)
            {
                var offset = smallerValue * (-1) + 1;
                largerValue += offset;
                smallerValue += offset;
            }
            
            _halfLifeCount = new HalfLifeCountCalculator(largerValue, smallerValue, PeriodFromStartToEnd);
        }        
        
        public BigInteger GetDeltaForGivenTime(BigInteger currentValue, double timePassed)
        {
            var result = _halfLifeCount.CalculateDecayed(currentValue, timePassed);
            return result;
        }
    }
}