namespace Sequence
{
    public struct SequenceContext
    {
        public SequenceContext(float length, int initialValue, int numberOfOperations)
        {
            Length = length;
            InitialValue = initialValue;
            NumberOfOperations = numberOfOperations;
        }
        
        public float Length {get; private set;}
        public int InitialValue {get; private set;}
        public int NumberOfOperations {get; private set;}
    }
}