using System.Numerics;
using System;
using System.Collections.Generic;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationDelegates : IOperationDelegates
    {
        static readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> _delegates = new Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>>(){
                    {Operation.Add, Addition},
                    {Operation.Subtract, Subtraction},
                    {Operation.Multiply, Multiplication},
                    {Operation.Divide, Division},
                    {Operation.Blank, Keep}};  
        
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action)
            => _delegates[action];
        
        static BigInteger Addition(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Add(initialValue, operationValue);
        static BigInteger Subtraction(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Subtract(initialValue, operationValue);
        static BigInteger Multiplication(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Multiply(initialValue, operationValue);
        static BigInteger Division(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Divide(initialValue, operationValue);
        static BigInteger Keep(BigInteger initialValue, BigInteger operationValue)
            => initialValue;
    }
}