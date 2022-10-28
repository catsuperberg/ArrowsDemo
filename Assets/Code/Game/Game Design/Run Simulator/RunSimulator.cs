using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;

namespace Game.GameDesign
{
    public class RunSimulator
    {
        ISequenceCalculator _sequenceCalculator;
        ITargetProvider _targetGenerator;
        VirtualPlayer _player;
        
        public RunSimulator(ISequenceCalculator sequenceCalculator, ITargetProvider targetGenerator, VirtualPlayer player)
        {
            _sequenceCalculator = sequenceCalculator ?? throw new System.ArgumentNullException(nameof(sequenceCalculator));
            _targetGenerator = targetGenerator ?? throw new System.ArgumentNullException(nameof(targetGenerator));
            _player = player ?? throw new System.ArgumentNullException(nameof(player));
        }
        
        public RunData Simulate(SequenceContext generationContext)
        {
            var simContext = GenerateContext(generationContext);        
            return _player.PerformRunWithAdUntilSucessful(simContext);
        }
        
        SimulationContext GenerateContext(SequenceContext generationContext)
        {
            var targetScore = _sequenceCalculator.GetAverageSequenceResult(generationContext);            
            var sequence = _sequenceCalculator.GenerateSequence(targetScore, spreadPercentage: 15, generationContext);
            (int Min, int Max) targetCountRange = (1, MaxTargerCount(targetScore));  
            var targets = _targetGenerator.GetDataOnlyTargets(targetScore, targetCountRange);
            var secondsPerGate = (generationContext.Length/(float)generationContext.NumberOfOperations)/generationContext.ProjectileSpeed;            
            
            return new SimulationContext(sequence, targetScore, targets, generationContext.InitialValue, secondsPerGate);
        }
        
        int MaxTargerCount(System.Numerics.BigInteger score) // HACK copied from ArrowsRunthroughFactory
        {
            var value = (score > 20) ? 20 : (int)score - 1;        
            return value;
        }
    }
}