namespace Game.GameDesign
{    
    public enum GraphType 
    {        
        RewardPerRun,
        UpgradesPerRun,
        UpgradesPerReward,
        TimeToReward,
        UpgradesFormulaCurve
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
                case GraphType.UpgradesFormulaCurve: return "Upgrades at reward computed from formula";
                default: return "No label implemented";
            }
        }
    }
}