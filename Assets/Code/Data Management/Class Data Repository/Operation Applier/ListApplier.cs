using System;

namespace DataManagement
{
    public class ListApplier : IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation)
        {
            string result;
            switch(typeOfOperation)
            {
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