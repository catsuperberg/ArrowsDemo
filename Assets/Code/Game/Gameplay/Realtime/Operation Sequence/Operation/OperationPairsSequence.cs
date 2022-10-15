using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public readonly struct  OperationPairsSequence
    {
        public readonly BigInteger BestPossibleResult;
        public readonly IReadOnlyCollection<OperationPair> Sequence {get {return _sequence.ToList().AsReadOnly();}}
        private readonly IEnumerable<OperationPair> _sequence;
        
        public OperationPairsSequence(List<OperationPair> sequence, BigInteger generationTimeResult)
        {
            if(sequence.Count == 0)
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
            
            _sequence = sequence;            
            BestPossibleResult = generationTimeResult;
        }
        
        public OperationPairsSequence(IEnumerable<OperationPair> sequence, BigInteger generationTimeResult)
        {
            if(!sequence.Any())
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
                
            _sequence = sequence;            
            BestPossibleResult = generationTimeResult;
        }
        
        
        public override bool Equals(object obj) 
        {
            return obj is OperationPairsSequence &&
                BestPossibleResult == ((OperationPairsSequence)obj).BestPossibleResult &&
                _sequence.GetHashCode() == ((OperationPairsSequence)obj)._sequence.GetHashCode();
        }
        
        public bool Equals(OperationPairsSequence obj) 
        {
            return
                BestPossibleResult == obj.BestPossibleResult &&
                _sequence == obj._sequence;
        }
        
        public override int GetHashCode() 
        {
            return BestPossibleResult.GetHashCode() ^ _sequence.GetHashCode();
        }  
                
        public static bool operator ==(OperationPairsSequence i1, OperationPairsSequence i2) 
            => i1.Equals(i2);
            
        public static bool operator !=(OperationPairsSequence i1, OperationPairsSequence i2) 
            => !i1.Equals(i2);
    }
}