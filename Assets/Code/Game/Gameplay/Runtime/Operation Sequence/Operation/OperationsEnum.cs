namespace Game.Gameplay.Runtime.OperationSequence.Operation
{
    public enum Operation
    {   
        Multiply,
        Divide,
        Add,
        Subtract,
        Blank
    }
    
    static class OperationsMethods
    {
        public static string ToSymbol(this Operation operationType)
        {
            switch (operationType)
            {
                case Operation.Multiply:
                    return "*";
                case Operation.Divide:
                    return "/";
                case Operation.Add:
                    return "+";
                case Operation.Subtract:
                    return "-";
                case Operation.Blank:
                    return "";
                default:
                    return "No symbol for operationType: " + operationType.ToString();
            }
        }
    }
}