using System;

namespace DataManagement
{
    public class BoolApplier : IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            var baseNumber = Convert.ToBoolean(baseValue);
            var incrementNumber = Convert.ToBoolean(incrementValue);
            bool result;
            switch(typeOfOperation)
            {
                case OperationType.Increase:
                case OperationType.Decrease:
                    throw new NotImplementedException("Trying to increas/decrease bool value by bool value, should use OperationType.Replace");
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