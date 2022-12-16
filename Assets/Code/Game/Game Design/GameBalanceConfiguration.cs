namespace Game.GameDesign
{
    public class GameBalanceConfiguration
    {
        public struct Controlls
        {
            /// <summary> base on 1.0, more positive on > 1 </summary>
            public float OperationFrequencySwing;
            /// <summary> base on 1.0, more high values on > 1 </summary>
            public float OperationValuesSwing;
                    
            /// <summary> probably don't make smaller than around 50 </summary>
            public int CheapestUpgradeStartingPrice;
            /// <summary> base on 1.0, more positive on > 1 </summary>
            public float PriceIncreaseSteepness;
            /// <summary> base on 1.0, more positive on > 1 </summary>
            public float LatePriceIncreaseSpeedup;

            public Controlls(
                float operationFrequencySwing, float operationValuesSwing, int cheapestUpgradeStartingPrice, 
                float priceIncreaseSteepness, float latePriceIncreaseSpeedup)
            {
                OperationFrequencySwing = operationFrequencySwing;
                OperationValuesSwing = operationValuesSwing;
                CheapestUpgradeStartingPrice = cheapestUpgradeStartingPrice;
                PriceIncreaseSteepness = priceIncreaseSteepness;
                LatePriceIncreaseSpeedup = latePriceIncreaseSpeedup;
            }
            
            public Controlls(GameBalanceConfiguration configuration)
            {
                OperationFrequencySwing = configuration.OperationFrequencySwing;
                OperationValuesSwing = configuration.OperationValuesSwing;
                CheapestUpgradeStartingPrice = configuration.CheapestUpgradeStartingPrice;
                PriceIncreaseSteepness = configuration.PriceIncreaseSteepness;
                LatePriceIncreaseSpeedup = configuration.LatePriceIncreaseSpeedup;
            }
            
            public GameBalanceConfiguration ToConfiguration()
                => new GameBalanceConfiguration(
                    OperationFrequencySwing, OperationValuesSwing, CheapestUpgradeStartingPrice,
                    PriceIncreaseSteepness, LatePriceIncreaseSpeedup);
        }
        
        public const string MainConfigurationName = "GameBalanceConfiguration";
        
        /// <summary> base on 1.0, more positive on > 1 </summary>
        public readonly float OperationFrequencySwing;
        /// <summary> base on 1.0, more high values on > 1 </summary>
        public readonly float OperationValuesSwing;
                
        /// <summary> probably don't make smaller than around 50 </summary>
        public readonly int CheapestUpgradeStartingPrice;
        /// <summary> base on 1.0, more positive on > 1 </summary>
        public readonly float PriceIncreaseSteepness;
        /// <summary> base on 1.0, more positive on > 1 </summary>
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