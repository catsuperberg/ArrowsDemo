namespace Game.GameDesign
{
    public class GameBalanceConfiguration
    {
        public const string MainConfigurationName = "GameBalanceConfiguration";
        
        /// <summary> base on 1.0, more positive on > 1 </summary>
        public readonly float OperationFrequencySwing;
        /// <summary> base on 1.0, more high values on > 1 </summary>
        public readonly float OperationValuesSwing;
        
        public readonly int CheapestUpgradeStartingPrice;
        public readonly float PriceIncreaseSteepness;
        public readonly float LatePriceIncreaseSpeedup;

        public GameBalanceConfiguration(
            float operationFrequencySwing, float operationValuesSwing, int cheapestUpgradeStartingPrice, 
            float priceIncreaseSteepness, float latePriceIncreaseSpeedup)
        {
            OperationFrequencySwing = operationFrequencySwing;
            OperationValuesSwing = operationValuesSwing;
            CheapestUpgradeStartingPrice = cheapestUpgradeStartingPrice;
            PriceIncreaseSteepness = priceIncreaseSteepness;
            LatePriceIncreaseSpeedup = latePriceIncreaseSpeedup;
        }
    }
}