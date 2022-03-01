using System;

namespace DataManagement
{
    public class FloatApplier : IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            var baseNumber = Convert.ToSingle(baseValue);
            var incrementNumber = Convert.ToSingle(incrementValue);
            float result;
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