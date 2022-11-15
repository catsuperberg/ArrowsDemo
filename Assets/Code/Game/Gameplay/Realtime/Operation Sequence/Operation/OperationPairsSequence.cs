using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationPairsSequence
    {
        public BigInteger CalculateResult() 
        {
            var accumulator = InitValue;
            for(int i = 0; i < Length; i++)
                accumulator = _sequence.ElementAt(i).BestResult(accumulator);
            BestPossibleResult = accumulator;
            return BestPossibleResult;
        }
        
        public readonly int Length;
        public IReadOnlyCollection<OperationPair> Sequence {get {return _sequence.ToList().AsReadOnly();}}
        readonly IEnumerable<OperationPair> _sequence;
        public readonly BigInteger InitValue;
        public BigInteger BestPossibleResult {get; private set;}
        
        public OperationPairsSequence(IEnumerable<OperationPair> sequence, BigInteger initValue)
        {  
            Length = sequence.Count();    
            if(Length == 0)
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
            
            _sequence = sequence;   
            InitValue = initValue;   
            BestPossibleResult = -1;
        }
        
        public override bool Equals(object obj) 
        {
            return obj is OperationPairsSequence &&
                // BestPossibleResult == ((OperationPairsSequence)obj).BestPossibleResult &&
                _sequence.GetHashCode() == ((OperationPairsSequence)obj)._sequence.GetHashCode();
        }
        
        public bool Equals(OperationPairsSequence obj) 
        {
            return
                // BestPossibleResult == obj.BestPossibleResult &&
                _sequence == obj._sequence;
        }
        
        public override int GetHashCode() 
        {
            return InitValue.GetHashCode() ^ _sequence.GetHashCode();
        }  
                
        public static bool operator ==(OperationPairsSequence i1, OperationPairsSequence i2) 
            => i1.Equals(i2);
            
        public static bool operator !=(OperationPairsSequence i1, OperationPairsSequence i2) 
            => !i1.Equals(i2);
    }
}