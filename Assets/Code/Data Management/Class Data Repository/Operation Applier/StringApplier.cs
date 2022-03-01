using System;

namespace DataManagement
{
    public class StringApplier : IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            string result;
            switch(typeOfOperation)
            {
                case OperationType.Increase:
                    result = baseValue + incrementValue;
                    break;
                case OperationType.Replace:
                    result = incrementValue;
                    break;
                default:
                    throw new ArgumentException("No such operation as: " + typeOfOperation + "found in class: " + this.GetType().Name);
            }
            return result;
        }
    }
}