using System;

namespace DataManagement
{
    public class IntApplier : IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            var baseNumber = Convert.ToInt32(baseValue);
            var incrementNumber = Convert.ToInt32(incrementValue);
            int result;
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