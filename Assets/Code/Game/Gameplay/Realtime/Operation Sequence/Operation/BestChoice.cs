namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public enum BestChoice
    {
        Left = -1,
        Right = 1,
        Both = 0
    }  
    
    public static class BestChoiceExtensions
    {
        public static BestChoice Oposite(this BestChoice value)
            => (BestChoice)((int)value * -1);
    }
}