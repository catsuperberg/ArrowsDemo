using ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GameMath
{
    public static class RandomBigIntListWithSetSum
    {        
        static BigInteger _zero = BigInteger.Zero;
        static BigInteger _one = BigInteger.One;
        
        public static List<BigInteger> Generate(BigInteger sum, int size, (float min, float max) spreadDeviation)
        {            
            var meanValue = sum/size;        
            var values = GenerateRandomValues(size, meanValue, spreadDeviation);             
            CorrectValuesToMeetSum(values, sum);    
            return values.ToList();
        }
        
        static BigInteger[] GenerateRandomValues(int size, BigInteger meanValue, (float min, float max) spreadRange)
        {
            var spreadValues = Enumerable.Range(0, size)
                .Select(entry => GlobalRandom.RandomDouble(spreadRange));
            return spreadValues
                .Select(spread => meanValue + (meanValue.multiplyByFractionFast(spread) * MathUtils.RandomSign()))
                .ToArray();
        }
        
        static void CorrectValuesToMeetSum(BigInteger[] values, BigInteger result)
        {   
            CorrectByFraction(values, result);           
            CorrectByIncrement(values, result); 
        }
        
        static void CorrectByFraction(BigInteger[] values, BigInteger result)
        {                
            var sum = values.Aggregate(BigInteger.Add); 
            var resultDeviation = System.Math.Exp(BigInteger.Log(result) - BigInteger.Log(sum));  
            for(int i = 0; i < values.Length; i++)
            {
                var newValue = values[i].multiplyByFractionFast(resultDeviation);
                values[i] = newValue.Sign>0 ? newValue : _one;
            }
            // return values 
            //     .Select(value => value.multiplyByFractionFast(resultDeviation))
            //     .Select(value => (value < 1) ? 1 : value); 
        }
        
        static void CorrectByIncrement(BigInteger[] values, BigInteger result)
        {            
            var sum = values.Aggregate(BigInteger.Add);
            var errorToCorrect = result-sum;
            for(int i = 0; i < values.Length; i++)
                values[i] = ChangeByAmount(values[i], ref errorToCorrect);
            // return values
            //     .Select(entry => ChangeByAmount(entry, ref errorToCorrect)); 
        }
        
        static BigInteger ChangeByAmount(BigInteger value, ref BigInteger ammount)
        {
            var newValue = value + ammount;
            ammount = _zero;
            if(newValue.Sign > 0)
                return newValue;
            
            ammount = _one + value;
            return _one;
        }
    }
}