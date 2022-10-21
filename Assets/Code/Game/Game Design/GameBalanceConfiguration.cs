namespace GameDesign
{
    public class GameBalanceConfiguration
    {
        public const string MainConfigurationName = "GameBalanceConfiguration";
        
        /// <summary> base on 1.0, more positive on > 1 </summary>
        public readonly float OperationFrequencySwing;
        /// <summary> base on 1.0, more high values on > 1 </summary>
        public readonly float OperationValuesSwing;

        public GameBalanceConfiguration(float operationFrequencySwing, float operationValuesSwing)
        {
            OperationFrequencySwing = operationFrequencySwing;
            OperationValuesSwing = operationValuesSwing;
        }
    }
}