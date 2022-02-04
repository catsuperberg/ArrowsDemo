using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationPair
    {
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
        }
        
        ///<summary> Creates operation pair with random operation instances </summary>
        public OperationPair()
        {        
            LeftOperation = new OperationInstance();
            RightOperation = new OperationInstance();   
        }
        
        public OperationInstance BestOperation(BigInteger initialValue, OperationExecutor exec)
        {
            var leftIsBest = exec.Perform(LeftOperation, initialValue) > exec.Perform(RightOperation, initialValue);
            return (leftIsBest) ? LeftOperation : RightOperation;
        }
        public BigInteger BestOperationResult(BigInteger initialValue, OperationExecutor exec)
        {
            var leftValue = exec.Perform(LeftOperation, initialValue);
            var rightValue =  exec.Perform(RightOperation, initialValue);
            return (leftValue > rightValue) ? leftValue : rightValue;
        }
    }
}
