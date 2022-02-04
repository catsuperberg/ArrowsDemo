using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationExecutor
    {
        public BigInteger Perform(OperationInstance action, BigInteger inputValue)
        {
            switch (action.Type)
            {
                case Operation.Add:                    
                    return BigInteger.Add(inputValue, new BigInteger(action.Value));
                case Operation.Subtract:
                    return BigInteger.Subtract(inputValue, new BigInteger(action.Value));       
                case Operation.Multiply:
                    return BigInteger.Multiply(inputValue, new BigInteger(action.Value));
                case Operation.Divide:        
                    return BigInteger.Divide(inputValue, new BigInteger(action.Value));      
                case Operation.Blank:
                    return inputValue;
                default:
                    return inputValue;
            }
        }
    }
}