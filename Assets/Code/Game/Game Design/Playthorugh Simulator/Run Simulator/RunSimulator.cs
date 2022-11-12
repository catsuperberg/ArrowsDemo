using ExtensionMethods;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using GameMath;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public class RunSimulator
    {        
        const float _finishingSceneSeconds = 3; // HACK copied from FinishingScene  
        const float _frameTimeSeconds = 0.016f;     
        
        ISequenceCalculator _sequenceCalculator;
        ITargetProvider _targetGenerator;
        
        public RunSimulator(ISequenceCalculator sequenceCalculator, ITargetProvider targetGenerator)
        {
            _sequenceCalculator = sequenceCalculator ?? throw new System.ArgumentNullException(nameof(sequenceCalculator));
            _targetGenerator = targetGenerator ?? throw new System.ArgumentNullException(nameof(targetGenerator));
        }
        
        public RunData Simulate(SequenceContext generationContext, PlayerActors actors)
        {
            var simContext = GenerateContext(generationContext);        
            return PerformRunWithAdUntilSucessful(simContext, actors);
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
        
        
        
        RunData PerformRunWithAdUntilSucessful(SimulationContext context, PlayerActors actors)
        {
            var gatesTaken = 0;
            BigInteger reward = 0;
            do
            {
                var runthroughResult = SingleRunthrough(context, actors.GateSelector);
                reward = runthroughResult.reward;
                gatesTaken += runthroughResult.gatesTaken;
            } while (reward <= 0);
            
            var secondsToFinish = gatesTaken*context.SecondsPerGate;
            
            reward = ApplyTargetMultipliers(reward, context);
            secondsToFinish += _finishingSceneSeconds;
                                   
            var ad = actors.AdSelector.AccountForAd();
            var adSeconds = ad.SecondsToWatch;            
            var finalScore = reward * new BigInteger(ad.Multiplier);
            
            var levelRunTime = context.SecondsPerGate * context.Sequence.Sequence.Count();
            
            return new RunData(context.TargetScore, context.Sequence.BestPossibleResult(), finalScore, secondsToFinish, levelRunTime, adSeconds);
        }
        
        (BigInteger reward, int gatesTaken) SingleRunthrough(SimulationContext context, GateSelector selector)
        {
            var runReward = context.InitialValue;
            var gateCount = 0;
            foreach(var pair in context.Sequence.Sequence)
            {
                gateCount++;
                var opertaion = selector.Choose(pair, runReward);
                runReward = opertaion.Perform(runReward);
                if(runReward <= 0)
                    break;
            }
            
            return (runReward, gateCount);
        }
        
        BigInteger ApplyTargetMultipliers(BigInteger rewardBeforeTargets, SimulationContext context)
        {
            var newReward = rewardBeforeTargets;
            var damageCalculator = new ExponentialCountCalculator(rewardBeforeTargets, 0, _finishingSceneSeconds);
            var damagePool = rewardBeforeTargets;
            var targetPool = context.Targets;
            
            while (damagePool > 0 && targetPool.Any())
            {
                var emptyTargets = false;
                var damage = damageCalculator.GetDeltaForGivenTime(damagePool, _frameTimeSeconds);
                
                foreach(var target in targetPool)
                {
                    if(target.Points <= damage)
                    {
                        damagePool -= target.Points;
                        damage -= target.Points;
                        target.Damage(target.Points);
                        newReward = newReward.multiplyByFraction(target.Grade.RewardMultiplier());
                        emptyTargets = true;
                    }
                    else
                    {                        
                        damagePool -= target.Points;
                        target.Damage(damage);
                        damage = 0;
                    }
                    if(damage <= 0)
                        break;
                }
                
                if (emptyTargets)                    
                    targetPool = targetPool.Where(target => target.Points > 0).ToList();
            }
            
            return newReward;
        }       
        
        public class Factory : Zenject.PlaceholderFactory<RunSimulator>
        {
        } 
    }
}