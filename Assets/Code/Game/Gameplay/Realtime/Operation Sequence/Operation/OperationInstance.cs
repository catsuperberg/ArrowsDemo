using System;
using Utils;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationInstance
    {                    
        public readonly Operation Type;
        public readonly int Value;    
        
        public static OperationInstance _blankInstance = new OperationInstance(Operation.Blank, 0);
        public static OperationInstance blank {get => _blankInstance;}    
                
        public OperationInstance(Operation type, int value)
        {
            if(!EnumUtils.InRange((int)type, (int)Operation.First, (int)Operation.Last))
                throw new System.Exception("no valid type provided on OperationInstance creation, type was: " + type);
            Type = type;
            Value = value;
        }
    }
}
