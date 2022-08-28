namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public struct SequenceContext
    {
        public SequenceContext(float length, int initialValue, int numberOfOperations, float projectileSpeed)
        {
            Length = length;
            InitialValue = initialValue;
            NumberOfOperations = numberOfOperations;
            ProjectileSpeed = projectileSpeed;
        }
        
        public float Length {get; private set;}
        public int InitialValue {get; private set;}
        public int NumberOfOperations {get; private set;}
        public float ProjectileSpeed {get; private set;}
    }
}