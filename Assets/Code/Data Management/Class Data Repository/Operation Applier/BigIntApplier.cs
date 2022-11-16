using System;
using System.Numerics;

namespace DataManagement
{
    public class BigIntApplier : IOperationApplier
    {
        static BigInteger _zero = BigInteger.Zero; //HACK original properties construct new BigInteger every time
        
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            var baseNumber = BigInteger.Parse(baseValue);
            var incrementNumber = BigInteger.Parse(incrementValue);
            BigInteger result;
            switch(typeOfOperation)
            {
                case OperationType.Increase:
                    result = baseNumber + incrementNumber;
                    break;
                case OperationType.Decrease:
                    result = baseNumber - incrementNumber;
                    break;
                case OperationType.Replace:
                    result = incrementNumber;
                    break;
                default:
                    throw new ArgumentException("No such operation as: " + typeOfOperation + "found in class: " + this.GetType().Name);
            }
            return result.ToString();
        }
    }    
}