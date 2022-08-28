namespace DataManagement
{
    public interface IOperationApplier
    {
        public string GetResultOfOperation(string baseValue, string incrementValue, OperationType typeOfOperation);
    } 
}