namespace Game.GameDesign
{    
    public enum GraphType 
    {        
        RewardPerRun,
        UpgradesPerRun,
        TimeToReward
    }
    
    public static class GraphTypeExtensions
    {
        public static string Label(this GraphType type)
        {
            switch(type)
            {
                case GraphType.RewardPerRun: return "Average reward at certain run";
                case GraphType.UpgradesPerRun: return "Average number of Upgrades at certain run";
                default: return "No label implemented";
            }
        }
    }
}