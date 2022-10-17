using System.Numerics;
using System;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public interface IOperationRules
    {
        public int MinInitless {get;}
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action);
        public int GetValueForType(Operation type, float stdSqrt, float stdSin, float coeff);
        // public (int min, int max) GetRange(int actionInt);
        public BestChoice ChooseBest(int leftIdentifier, int rightIdentifier, int initialValue);
        public BestChoice ChooseFastBest(int leftIdentifier, int rightIdentifier);
        // public BestChoice ChooseFastBest(OperationInstance leftIdentifier, OperationInstance rightIdentifier);
    }
}