using System;
using System.Numerics;
using Utils;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public readonly struct OperationInstance
    {                    
        public readonly Operation Type;
        public readonly BigInteger Value;   
        public readonly int Identifier; 
        
        readonly Func<BigInteger, BigInteger, BigInteger> _execute;
        
        public static OperationInstance _blankInstance = new OperationInstance(Operation.Blank, 0, (BigInteger i1, BigInteger i2) => {return 0;});
        public static OperationInstance blank {get => _blankInstance;}    
                
        public OperationInstance(Operation type, int value, IOperationRules delegates)
        {
            if(!EnumUtils.InRange((int)type, (int)Operation.First, (int)Operation.Last))
                throw new System.Exception("no valid type provided on OperationInstance creation, type was: " + type);
            Type = type;
            Value = value;
            _execute = delegates.GetDelegate(Type);
            Identifier = (((int)type) << 4) | value; // HACK shift by 5 bits because max value is 10 and fits in 4 bits;
        }
        
        public OperationInstance(Operation type, int value, Func<BigInteger, BigInteger, BigInteger> delegatedFunction)
        {
            if(!EnumUtils.InRange((int)type, (int)Operation.First, (int)Operation.Last))
                throw new System.Exception("no valid type provided on OperationInstance creation, type was: " + type);
            Type = type;
            Value = value;
            _execute = delegatedFunction;
            Identifier = (((int)type) << 4) | value; // HACK shift by 5 bits because max value is 10 and fits in 4 bits;
        }                  
        
        public BigInteger Perform(BigInteger initialValue)
            => _execute(initialValue, Value);            
                
        
        public override bool Equals(object obj) 
        {
            return obj is OperationInstance &&
                Identifier == ((OperationInstance)obj).Identifier;
        }
        
        public bool Equals(OperationInstance obj) 
        {
            return
                Identifier == obj.Identifier;
        }
        
        public override int GetHashCode() 
        {
            return (int)Type ^ (int)Value;
        }  
                
        public static bool operator ==(OperationInstance i1, OperationInstance i2) 
            => i1.Equals(i2);
            
        public static bool operator !=(OperationInstance i1, OperationInstance i2) 
            => !i1.Equals(i2);
    }
}
