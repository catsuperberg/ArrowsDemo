using System;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public readonly struct OperationPair
    {
        public readonly OperationInstance LeftOperation;
        public readonly OperationInstance RightOperation;
        readonly IOperationRules _rules;
        readonly BestChoice _best;
        
        public OperationPair(OperationInstance left, OperationInstance right, IOperationRules rules)
        {        
            LeftOperation = left;
            RightOperation = right;   
            _rules = rules;
            _best = rules.ChooseFastBest(left.Identifier, right.Identifier);   
        }                     
        
        public BigInteger BestResult(BigInteger initialValue)
        {
            var best = (initialValue >= _rules.MinInitless) ? _best : _rules.ChooseBest(LeftOperation.Identifier, RightOperation.Identifier, initialValue);
            return ResultByChoice(best, initialValue);
        }
        
        public BigInteger ResultByChoice(BestChoice choice, BigInteger initialValue)       
        {
            if(choice == BestChoice.Right)
                return RightOperation.Perform(initialValue);
            return LeftOperation.Perform(initialValue);
        }       
        
        public OperationInstance OperationByChoice(BestChoice choice)        
        {
            if(choice == BestChoice.Right)
                return RightOperation;
            return LeftOperation;
        }        
        
        public OperationInstance WorseOperation(BigInteger initialValue)
        {
            if(initialValue >= _rules.MinInitless)
                return OperationByChoice(_best.Oposite());
            return OperationByChoice(_rules.ChooseBest(LeftOperation.Identifier, RightOperation.Identifier, initialValue).Oposite());
        }
        
        
        public OperationInstance BestOperation(BigInteger initialValue)
        {
            if(initialValue >= _rules.MinInitless)
                return OperationByChoice(_best);
            return OperationByChoice(_rules.ChooseBest(LeftOperation.Identifier, RightOperation.Identifier, initialValue));
        }        
        
        
        public bool IsBestOperation(OperationInstance operationToCheck, BigInteger initialValue)
        {
            OperationInstance otherOperation;
            if(operationToCheck == LeftOperation)
                otherOperation = RightOperation;
            else if(operationToCheck == RightOperation)
                otherOperation = LeftOperation;
            else
                throw new Exception("No OperationInstance, equal to one to check, found in this pair");
            return operationToCheck.Perform(initialValue) >= otherOperation.Perform(initialValue);
        }
        
        
        public override bool Equals(object obj) 
        {
            return obj is OperationPair &&
                LeftOperation.Identifier == ((OperationPair)obj).LeftOperation.Identifier;
        }
        
        public bool Equals(OperationPair obj) 
        {
            return LeftOperation.Identifier == obj.RightOperation.Identifier;
        }
        
        public override int GetHashCode() 
        {
            return LeftOperation.Identifier ^ RightOperation.Identifier;
        }
        
        public static bool operator ==(OperationPair x, OperationPair y) 
        {
            return x.Equals(y);
        }
        public static bool operator !=(OperationPair x, OperationPair y) 
        {
            return !x.Equals(y);
        }
    }
}
