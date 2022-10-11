using System.Numerics;
using System;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public interface IOperationDelegates
    {
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action);
    }
}