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
                    return BigInteger.Subtract(inputValue, new BigInteger(action.value));
                case Operations.Multiply:
                    return BigInteger.Multiply(inputValue, new BigInteger(action.value));
                case Operations.Divide:
                    return BigInteger.Divide(inputValue, new BigInteger(action.value));       
                case Operations.Blank:
                    return inputValue;
                default:
                    return inputValue;
            }
        }
    }
}