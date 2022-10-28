using ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GameMath
{
    public static class RandomBigIntListWithSetSum
    {        
        public static List<BigInteger> Generate(BigInteger sum, int size, (float min, float max) spreadDeviation)
        {            
            var meanValue = sum/size;        
            List<BigInteger> values = GenerateRandomValues(size, meanValue, spreadDeviation);             
            values = CorrectValuesToMeetSum(values, sum);    
            return values;
        }
        
        static List<BigInteger> GenerateRandomValues(int size, BigInteger meanValue, (float min, float max) spreadRange)
            => Enumerable.Range(0, size)
                .Select(entry => meanValue + new BigInteger(GlobalRandom.RandomDouble(spreadRange) * MathUtils.RandomSign()) )
                .ToList();
        
        static List<BigInteger> CorrectValuesToMeetSum(List<BigInteger> values, BigInteger result)
        {
            var tempValues = values;    
            tempValues = CorrectByFraction(tempValues, result);           
            tempValues = CorrectByIncrement(tempValues, result); 
            return tempValues;
        }
        
        static List<BigInteger> CorrectByFraction(List<BigInteger> values, BigInteger result)
        {                
            var sum = values.Aggregate(BigInteger.Add); 
            var resultDeviation = System.Math.Exp(BigInteger.Log(result) - BigInteger.Log(sum));  
            return values.Select(value => value.multiplyByFraction(resultDeviation)).Select(value => (value < 1) ? 1 : value).ToList(); 
        }
        
        static List<BigInteger> CorrectByIncrement(List<BigInteger> values, BigInteger result)
        {            
            var sum = values.Aggregate(BigInteger.Add);
            var errorToCorrect = result-sum;
            return values.Select(entry => ChangeByAmount(entry, ref errorToCorrect)).ToList(); 
        }
        
        static BigInteger ChangeByAmount(BigInteger value, ref BigInteger ammount)
        {
            var newValue = value + ammount;
            ammount = 0;
            if(newValue > 0)
                return newValue;
            
            ammount = 1 + value;
            return 1;
        }
    }
}