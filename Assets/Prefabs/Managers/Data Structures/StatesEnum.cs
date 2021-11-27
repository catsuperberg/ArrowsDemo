namespace State
{
    public enum States
    {   
        StartScreen,
        GamePlay,
        Menu,
        FinishingCutscene,
        PreAdTease,
        Ad
    }
    
    // static class OperationsMethods
    // {
    //     public static string ToSymbol(this Operations operationType)
    //     {
    //         switch (operationType)
    //         {
    //             case Operations.Multiply:
    //                 return "*";
    //             case Operations.Divide:
    //                 return "/";
    //             case Operations.Add:
    //                 return "+";
    //             case Operations.Subtract:
    //                 return "-";
    //             case Operations.Blank:
    //                 return "";
    //             default:
    //                 return "No symbol for operationType: " + operationType.ToString();
    //         }
    //     }
    // }
}