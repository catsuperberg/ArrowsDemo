using System.Numerics;

namespace Sequence
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
        
        public OperationInstance BestOperation(BigInteger initialValue, OperationExecutor exec)
        {
            var LeftIsBest = exec.Perform(LeftOperation, initialValue) > exec.Perform(RightOperation, initialValue);
            return (LeftIsBest) ? LeftOperation : RightOperation;
        }
    }
}
