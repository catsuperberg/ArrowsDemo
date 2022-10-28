namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class SequenceContext
    {
        public readonly float Length;
        public readonly int InitialValue;
        public readonly int NumberOfOperations;
        public readonly float ProjectileSpeed;
        
        public SequenceContext(float length, int initialValue, int numberOfOperations, float projectileSpeed)
        {
            Length = length;
            InitialValue = initialValue;
            NumberOfOperations = numberOfOperations;
            ProjectileSpeed = projectileSpeed;
        }
    }
}