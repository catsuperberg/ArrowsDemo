using System;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public readonly struct OperationPair
    {
        const int _minFastValue = 18; // HACK because of max subtract 10 and min devision 2, after 18 FastLeftIsBest always works
        
        public readonly OperationInstance LeftOperation;
        public readonly OperationInstance RightOperation;
        readonly bool _leftIsBest;
        
        public OperationPair(OperationInstance left, OperationInstance right)
        {        
            LeftOperation = left;
            RightOperation = right;   
            _leftIsBest = FastLeftIsBest(LeftOperation, RightOperation);   
        }
                 
        
        static bool FastLeftIsBest(OperationInstance left, OperationInstance right)
        {
            var letfType = ToValue((int)left.Type);
            var rightType = ToValue((int)right.Type);
            var typeResult = Math.Sign(rightType - letfType);
            var dontSkip = TrueOnZero(typeResult);
            var equalResult = SighnReverseOnNegative(Operation.Add - left.Type) ^ ((int)left.Value - (int)right.Value);
            return Convert.ToBoolean(ZeroOnNegative(typeResult + (dontSkip & equalResult)));
        }
        
        public static int ToValue(int operationType)
            => operationType - (((operationType & 1) & (operationType >> 2))<<1);
            // return (value & TrueOnNegative(value-5)) + (3 & TrueOnNegative(4-value));
        
        static int ZeroOnNegative(int value)
            => (value >> 31) + 1;               
            
        static int TrueOnNegative(int value)
            => value >> 31;      
                    
        static int TrueOnZero(int value)
            => ((value ^ 1) & 1) << 31 >> 31;   
            
        static int SighnWithoutZero(int value)
            => (value >> 31) | 0b01;    
            
        static int SighnReverseOnNegative(int value)
        {
            unchecked {return (int)(((uint)value) & 0x8000_0000);}; 
        }     
        
        public OperationInstance BestOperation(BigInteger initialValue)
        {
            if(initialValue > _minFastValue)
                return FastBest();
            return FullBestOperation(initialValue);
        }        
        
        public OperationInstance FullBestOperation(BigInteger initialValue)
        {
            var leftIsBest = LeftOperation.Perform(initialValue) > RightOperation.Perform(initialValue);
            return (leftIsBest) ? LeftOperation : RightOperation;
        }        
        
        public BigInteger BestOperationWithResult(BigInteger initialValue)
        {
            if(initialValue > _minFastValue)
                return FastBest().Perform(initialValue);
            return FullBestOperationWithResult(initialValue);
        }
        
        public BigInteger FullBestOperationWithResult(BigInteger initialValue)
        {
            var leftResult = LeftOperation.Perform(initialValue);
            var rightResult = RightOperation.Perform(initialValue);
            return (leftResult > rightResult) ? leftResult : rightResult;
        }
        
        public OperationInstance FastBest()
            => (_leftIsBest) ? LeftOperation : RightOperation;
        
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
                throw new Exception("No OperationInstance, equal to one to check, found in this pair");
            return operationToCheck.Perform(initialValue) >= otherOperation.Perform(initialValue);
        }
        
        public BigInteger BestOperationResult(BigInteger initialValue)
            => BestOperationWithResult(initialValue);
        
        public override bool Equals(object obj) 
        {
            return obj is OperationPair &&
                LeftOperation.Type == ((OperationPair)obj).LeftOperation.Type &&
                LeftOperation.Value == ((OperationPair)obj).LeftOperation.Value &&
                RightOperation.Type == ((OperationPair)obj).RightOperation.Type &&
                RightOperation.Value == ((OperationPair)obj).RightOperation.Value;
        }
        
        public bool Equals(OperationPair obj) 
        {
            return
                LeftOperation.Type == obj.LeftOperation.Type &&
                LeftOperation.Value == obj.LeftOperation.Value &&
                RightOperation.Type == obj.RightOperation.Type &&
                RightOperation.Value == obj.RightOperation.Value;
        }
        
        public override int GetHashCode() 
        {
            return LeftOperation.GetHashCode() ^ RightOperation.GetHashCode();
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
