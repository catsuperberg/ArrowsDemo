namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public enum Operation
    {   
        Multiply = 1,
        Add = 2,
        Subtract = 3,
        Divide = 4,
        Blank = 5,
        First = Multiply,
        Last = Blank
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
        
        public static int ToOffset(this Operation operationType, int perOffsetSize)
        {
            return ((int)operationType-1)*perOffsetSize;
        }
        
        // public static int ToValue(this Operation operationType)
        // {
        //     var value = (int)operationType;
        //     return value * (((4-value).GetHashCode() >> 31) + 1) + 3 * (((value-5).GetHashCode() >> 31) + 1);
        // }
    }
}