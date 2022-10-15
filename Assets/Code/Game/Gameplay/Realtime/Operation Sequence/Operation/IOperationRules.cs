using System.Numerics;
using System;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public interface IOperationRules
    {
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action);
        public (int min, int max) GetRange(Operation action);
    }
}