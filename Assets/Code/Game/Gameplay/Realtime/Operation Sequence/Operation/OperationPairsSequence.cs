using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public readonly struct  OperationPairsSequence
    {
        public readonly BigInteger BestPossibleResult () 
        {
            var accumulator = _initValue;
            for(int i = 0; i < _sequence.Length; i++)
                accumulator = _sequence[i].BestResult(accumulator);
            return accumulator;
        }
        
        public readonly int Length;
        public readonly IReadOnlyCollection<OperationPair> Sequence {get {return _sequence.ToList().AsReadOnly();}}
        readonly OperationPair[] _sequence;
        readonly BigInteger _initValue;
        
        public OperationPairsSequence(OperationPair[] sequence, BigInteger initValue)
        {  
            Length = sequence.Length;    
            if(Length == 0)
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
            
            _sequence = sequence;   
            _initValue = initValue;   
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
            return _initValue.GetHashCode() ^ _sequence.GetHashCode();
        }  
                
        public static bool operator ==(OperationPairsSequence i1, OperationPairsSequence i2) 
            => i1.Equals(i2);
            
        public static bool operator !=(OperationPairsSequence i1, OperationPairsSequence i2) 
            => !i1.Equals(i2);
    }
}