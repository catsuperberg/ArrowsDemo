namespace Game.GameDesign
{    
    public enum SimValueType
    {        
        PlaythroughTime,
        GateSelectorStats,
        AdSelectorStats,        
        AveragePlaythroughTime
    }
    
    public static class ValueTypeExtensions
    {
        public static string Label(this SimValueType type)
        {
            switch(type)
            {
                case SimValueType.PlaythroughTime: return "Average time to completion";
                case SimValueType.GateSelectorStats: return "Average gate selector";
                case SimValueType.AdSelectorStats: return "Average ad selector";
                case SimValueType.AveragePlaythroughTime: return "Time to completion for average player";
                default: return "No label implemented";
            }
        }
    }
}