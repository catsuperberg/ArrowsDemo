namespace Sequence
{
    public enum Operations
    {   
        Multiply,
        Divide,
        Add,
        Subtract,
        Blank
    }
    
    static class OperationsMethods
    {
        public static string ToSymbol(this Operations operationType)
        {
            switch (operationType)
            {
                case Operations.Multiply:
                    return "*";
                case Operations.Divide:
                    return "/";
                case Operations.Add:
                    return "+";
                case Operations.Subtract:
                    return "-";
                case Operations.Blank:
                    return "";
                default:
                    return "No symbol for operationType: " + operationType.ToString();
            }
        }
    }
}