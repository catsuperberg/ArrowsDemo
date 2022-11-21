using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public readonly struct OperationPairsSequence : System.IComparable<OperationPairsSequence>
    {
        public readonly int Length;
        public IReadOnlyCollection<OperationPair> Sequence {get {return _sequence.ToList().AsReadOnly();}}
        readonly IEnumerable<OperationPair> _sequence;
        public readonly BigInteger InitValue;
        public readonly BigInteger BestPossibleResult;
        
        public OperationPairsSequence(IEnumerable<OperationPair> sequence, BigInteger initValue, BigInteger result)
        {  
            Length = sequence.Count();    
            if(Length == 0)
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
            
            _sequence = sequence;   
            InitValue = initValue;   
            BestPossibleResult = result;
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
            
        public int CompareTo(OperationPairsSequence incomingobject)
            => this.BestPossibleResult.CompareTo(incomingobject.BestPossibleResult);
    }
}