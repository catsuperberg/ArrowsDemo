namespace Game.GameDesign
{    
    public enum GraphType 
    {        
        RewardPerRun,
        UpgradesPerRun,
        UpgradesPerReward,
        TimeToReward,        
        
        AverageRewardPerRun,
        AverageUpgradesPerRun,
        AverageUpgradesPerReward,
        AverageTimeToReward,
        
        UpgradePricing
    }
    
    public static class GraphTypeExtensions
    {
        public static string Label(this GraphType type)
        {
            switch(type)
            {
                case GraphType.RewardPerRun: return "Average reward at certain run";
                case GraphType.UpgradesPerRun: return "Average number of Upgrades at certain run";
                case GraphType.UpgradesPerReward: return "Upgrades at reward";
                case GraphType.TimeToReward: return "Average time to get to reward level";
                
                case GraphType.AverageRewardPerRun: return "Reward at certain run for average player";
                case GraphType.AverageUpgradesPerRun: return "Number of Upgrades at certain run for average player";
                case GraphType.AverageUpgradesPerReward: return "Upgrades at reward for average player";
                case GraphType.AverageTimeToReward: return "Time to get to reward level for average player";                   
                
                case GraphType.UpgradePricing: return "Price progression for upgrades";               
                default: return "No label implemented";
            }
        }
    }
}