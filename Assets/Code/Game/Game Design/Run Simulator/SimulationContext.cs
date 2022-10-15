using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using System.Collections.Generic;
using System.Numerics;

namespace Game.GameDesign
{
    public struct SimulationContext
    {
        public readonly OperationPairsSequence Sequence;
        public readonly BigInteger TargetScore;
        public readonly List<TargetDataOnly> Targets;
        public readonly BigInteger InitialValue;
        public readonly float SecondsPerGate;

        public SimulationContext(
            OperationPairsSequence sequence, BigInteger targetScore, List<TargetDataOnly> targets, 
            BigInteger initialValue, float secondsPerGate)
        {
            Sequence = sequence;
            TargetScore = targetScore;
            Targets = targets ?? throw new System.ArgumentNullException(nameof(targets));
            SecondsPerGate = secondsPerGate;
            InitialValue = initialValue;
        }
    }
}