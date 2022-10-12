using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public struct OperationPair 
    {
        readonly OperationFactory _factory;
        
        public OperationInstance LeftOperation {get; private set;}
        public OperationInstance RightOperation {get; private set;}
        
        public OperationPair(OperationInstance left, OperationInstance right)
        {        
            if(left.Equals(default(OperationInstance)))
                throw new System.Exception("LeftOperation isn't provided to OpetationsPair");
            if(right.Equals(default(OperationInstance)))
                throw new System.Exception("RighOperation isn't provided to OpetationsPair");
                
            LeftOperation = left;
            RightOperation = right; 
            _factory = null;  
        }
        
        ///<summary> Creates operation pair with random operation instances </summary>
        public OperationPair(OperationFactory operationFactory)
        {                    
            _factory = operationFactory;   
            LeftOperation = _factory.GetRandom();
            RightOperation = _factory.GetRandom();
        }
        
        public void Regenerate()
        {
            LeftOperation = _factory.GetRandom();
            RightOperation = _factory.GetRandom();            
        }  
        
        public OperationInstance BestOperation(BigInteger initialValue)
        {
            var leftIsBest = LeftOperation.Perform(initialValue) > RightOperation.Perform(initialValue);
            return (leftIsBest) ? LeftOperation : RightOperation;
        }
        
        public OperationInstance WorseOperation(BigInteger initialValue)
        {
            var leftIsBest = LeftOperation.Perform(initialValue) > RightOperation.Perform(initialValue);
            return (leftIsBest) ? RightOperation : LeftOperation;
        }
        
        public bool IsBestOperation(OperationInstance operationToCheck, BigInteger initialValue)
        {
            OperationInstance otherOperation;
            if(operationToCheck == LeftOperation)
                otherOperation = RightOperation;
            else if(operationToCheck == RightOperation)
                otherOperation = LeftOperation;
            else
                throw new System.Exception("No OperationInstance, equal to one to check, found in this pair");
            return operationToCheck.Perform(initialValue) >= otherOperation.Perform(initialValue);
        }
        
        public BigInteger BestOperationResult(BigInteger initialValue)
        {
            var leftValue = LeftOperation.Perform(initialValue);
            var rightValue =  RightOperation.Perform(initialValue);
            return (leftValue > rightValue) ? leftValue : rightValue;
        }
    }
}
