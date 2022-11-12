using System.Numerics;
using System.Collections.Generic;
using System;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public interface IOperationRules
    {
        public BigInteger MinInitless {get;}
        IReadOnlyDictionary<Operation, int> OperationFrequencies{get;}
        IReadOnlyDictionary<Operation, int> OperationRepeats(int count);
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action);
        public int GetValueForType(Operation type, float randStdNormal);
        /// <summary> initialValue can't be higher or equal _minInitlessValue </summary>
        public BestChoice ChooseBest(int leftIdentifier, int rightIdentifier, BigInteger initialValue);
        public BestChoice ChooseFastBest(int leftIdentifier, int rightIdentifier);
    }
}