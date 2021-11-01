using System.Numerics;

namespace Sequence
{
    public class OperationPair
    {
        public OperationInstance LeftOperation {get; private set;}
        public OperationInstance RighOperation {get; private set;}
        public bool LeftIsBest {get; private set;}
        
        public OperationPair(OperationInstance left, OperationInstance right, OperationExecutor exec)
        {        
            if(left.Equals(default(OperationInstance)))
                throw new System.Exception("LeftOperation isn't provided to OpetationsPair");
            if(right.Equals(default(OperationInstance)))
                throw new System.Exception("RighOperation isn't provided to OpetationsPair");
                
            LeftOperation = left;
            RighOperation = right;   
            LeftIsBest = exec.Perform(LeftOperation, 1) > exec.Perform(RighOperation, 1);
        }
        
        public OperationInstance BestOperation()
        {
            return (LeftIsBest) ? LeftOperation : RighOperation;
        }
            
        public OperationInstance WorstOperation()
        {
            return (LeftIsBest) ? RighOperation : LeftOperation;
        }
    }
}
