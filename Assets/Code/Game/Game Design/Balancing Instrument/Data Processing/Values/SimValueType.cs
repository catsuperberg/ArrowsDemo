namespace Game.GameDesign
{    
    public enum SimValueType
    {        
        PlaythroughTime,
    }
    
    public static class ValueTypeExtensions
    {
        public static string Label(this SimValueType type)
        {
            switch(type)
            {
                case SimValueType.PlaythroughTime: return "Average time to completion";
                default: return "No label implemented";
            }
        }
    }
}