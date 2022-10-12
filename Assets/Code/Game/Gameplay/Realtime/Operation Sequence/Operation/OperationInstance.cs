using System;
using System.Numerics;
using Utils;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public struct OperationInstance
    {                    
        public readonly Operation Type;
        public int Value;    
        
        Func<BigInteger, BigInteger, BigInteger> _execute;
        
        public static OperationInstance _blankInstance = new OperationInstance(Operation.Blank, 0, new OperationDelegates());
        public static OperationInstance blank {get => _blankInstance;}    
                
        public OperationInstance(Operation type, int value, IOperationDelegates delegates)
        {
            if(!EnumUtils.InRange((int)type, (int)Operation.First, (int)Operation.Last))
                throw new System.Exception("no valid type provided on OperationInstance creation, type was: " + type);
            Type = type;
            Value = value;
            _execute = delegates.GetDelegate(Type);
        }
        
        public OperationInstance(Operation type, int value, Func<BigInteger, BigInteger, BigInteger> delegatedFunction)
        {
            if(!EnumUtils.InRange((int)type, (int)Operation.First, (int)Operation.Last))
                throw new System.Exception("no valid type provided on OperationInstance creation, type was: " + type);
            Type = type;
            Value = value;
            _execute = delegatedFunction;
        }
        
        public void Update(int value)
        {
            Value = value;
        }
                
        
        public BigInteger Perform(BigInteger initialValue)
            => _execute(initialValue, new BigInteger(Value));            
                
        public static bool operator ==(OperationInstance i1, OperationInstance i2) 
            => i1.Equals(i2);
            
        public static bool operator !=(OperationInstance i1, OperationInstance i2) 
            => !i1.Equals(i2);
    }
}
