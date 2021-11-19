using System.Numerics;

namespace Sequence
{
    public class OperationExecutor
    {
        public BigInteger Perform(OperationInstance action, BigInteger inputValue)
        {
            switch (action.operationType)
            {
                case Operations.Add:                    
                    return BigInteger.Add(inputValue, new BigInteger(action.value));
                case Operations.Subtract:
                    var value = BigInteger.Subtract(inputValue, new BigInteger(action.value));
                    if(value < 1) value = new BigInteger(1);
                    return value;
                case Operations.Multiply:
                    return BigInteger.Multiply(inputValue, new BigInteger(action.value));
                case Operations.Divide:                    
                    var value1 = BigInteger.Divide(inputValue, new BigInteger(action.value));
                    if(value1 < 1) value1 = new BigInteger(1);
                    return value;       
                case Operations.Blank:
                    return inputValue;
                default:
                    return inputValue;
            }
        }
    }
}