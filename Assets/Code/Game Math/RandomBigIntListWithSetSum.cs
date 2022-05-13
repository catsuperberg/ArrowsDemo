using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GameMath
{
    public static class RandomBigIntListWithSetSum
    {
        private const int fractionalCorrectionThreshold = 300;
        
        public static List<BigInteger> Generate(BigInteger sum, int size, (float min, float max) spreadDeviation)
        {            
            var meanValue = sum/size;        
            List<BigInteger> values = GenerateRandomValues(size, meanValue, spreadDeviation);             
            values = CorrectValuesToMakeSum(values, sum);    
            return values;
        }
        
        static List<BigInteger> GenerateRandomValues(int size, BigInteger meanValue, (float min, float max) spreadDeviation)
        {
            var values = new List<BigInteger>();
            for(int i = 0; i < size; i++)
            {
                var spread = meanValue.multiplyByFraction(
                    GlobalRandom.RandomDouble(spreadDeviation.min, spreadDeviation.max)) * new BigInteger(MathUtils.RandomSign());
                var value = meanValue+spread;
                values.Add(value);
            }
            return values;
        }        
        
        static List<BigInteger> CorrectValuesToMakeSum(List<BigInteger> values, BigInteger result)
        {
            var tempValues = values;    
            var timeStarted = DateTimeOffset.UtcNow.ToUnixTimeSeconds();        
            while(SumOfTargets(tempValues) != result)
            {
                var correctionAmmount = result-SumOfTargets(tempValues);                    
                if(result >= fractionalCorrectionThreshold && correctionAmmount >= fractionalCorrectionThreshold)
                    tempValues = CorrectByFraction(tempValues, result); 
                else
                    tempValues = CorrectByIncrement(tempValues, result, correctionAmmount); 
                
                if(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timeStarted >= 40)
                    throw new TimeoutException("Random BigInt list generation taking more than 40 seconds");
            }
            return tempValues;
        }
        
        static List<BigInteger> CorrectByFraction(List<BigInteger> values, BigInteger result)
        {       
            List<BigInteger> tempValues = new List<BigInteger>();           
            var resultDeviation = System.Math.Exp(BigInteger.Log(result) - BigInteger.Log(SumOfTargets(values)));  
            foreach(BigInteger entry in values)
            {
                var valueToAdd = entry.multiplyByFraction(resultDeviation);
                valueToAdd = (valueToAdd < 1) ? 1 : valueToAdd;
                tempValues.Add(entry.multiplyByFraction(resultDeviation));                
            }
            return tempValues;   
        }
        
        static List<BigInteger> CorrectByIncrement(List<BigInteger> values, BigInteger result, BigInteger errorToCorrect)
        {
            List<BigInteger> tempValues = new List<BigInteger>();          
            foreach(var value in values)  
            {
                if(errorToCorrect != 0)
                {
                    var valueWithError = value + errorToCorrect;
                    if(valueWithError >= 1)
                    {
                        errorToCorrect = 0;                            
                        tempValues.Add(valueWithError);
                    }
                    else if(valueWithError == 0)
                    {                        
                        errorToCorrect = -1;                            
                        tempValues.Add(1);
                    }
                    else
                    {
                        errorToCorrect = valueWithError - 1;             
                        tempValues.Add(1);
                    }
                }                
                else
                    tempValues.Add(value);
            }
            return tempValues;   
        }
                
        static BigInteger SumOfTargets(List<BigInteger> targetScores)
        {
            return targetScores.Aggregate((currentSum, item) => currentSum + item);
        }
    }
}