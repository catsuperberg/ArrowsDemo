using ExtensionMethods;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using GameMath;
using System.Numerics;

namespace Game.GameDesign
{
    public class RunSimulator
    {        
        BigInteger _zero = BigInteger.Zero; //HACK original properties construct new BigInteger every time
        
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
            var sequence = _sequenceCalculator.GetSequenceInSpreadRange(targetScore, spreadPercentage: 15, generationContext);
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
            var reward = _zero;
            do
            {
                var runthroughResult = SingleRunthrough(context, actors.GateSelector);
                reward = runthroughResult.reward;
                gatesTaken += runthroughResult.gatesTaken;
            } while (reward.Sign <= 0);
            
            var secondsToFinish = gatesTaken*context.SecondsPerGate;
            
            reward = ApplyTargetMultipliers(reward, context);
            secondsToFinish += _finishingSceneSeconds;
                                   
            var ad = actors.AdSelector.AccountForAd();
            var adSeconds = ad.SecondsToWatch;            
            var finalScore = reward * new BigInteger(ad.Multiplier);
            
            var levelRunTime = context.SecondsPerGate * context.Sequence.Length;
            
            return new RunData(context.TargetScore, context.Sequence.BestPossibleResult, finalScore, secondsToFinish, levelRunTime, adSeconds);
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
                if(runReward.Sign <= 0)
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
            var targetCount = targetPool.Length;
            
            while (damagePool.Sign > 0 && targetCount > 0)
            {
                var damage = damageCalculator.GetDeltaForGivenTime(damagePool, _frameTimeSeconds);
                
                for(int i = 0; i < targetPool.Length; i++)
                {
                    if(targetPool[i].Points.Sign <= 0)
                        continue;
                    if(targetPool[i].Points <= damage)
                    {
                        damagePool -= targetPool[i].Points;
                        damage -= targetPool[i].Points;
                        targetPool[i].Damage(targetPool[i].Points);
                        newReward = newReward.multiplyByFractionFast(targetPool[i].Grade.RewardMultiplier());
                        targetCount--;
                    }
                    else
                    {                        
                        damagePool -= targetPool[i].Points;
                        targetPool[i].Damage(damage);
                        damage = _zero;
                    }
                    if(damage.Sign <= 0)
                        break;
                }
            }
            
            return newReward;
        }       
        
        public class Factory : Zenject.PlaceholderFactory<RunSimulator>
        {
        } 
    }
}