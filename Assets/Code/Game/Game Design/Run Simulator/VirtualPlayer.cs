using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using GameMath;
using ExtensionMethods;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public class VirtualPlayer
    {        
        GateSelector _gateSelector;
        IAdSelector _adSelector;
        
        const float _finishingSceneSeconds = 3; // HACK copied from FinishingScene  
        const float _frameTimeSeconds = 0.016f;     
        
        public VirtualPlayer(GateSelector gateSelector, IAdSelector adSelector)
        {
            _gateSelector = gateSelector ?? throw new System.ArgumentNullException(nameof(gateSelector));
            _adSelector = adSelector ?? throw new System.ArgumentNullException(nameof(adSelector));
        }
        
        public SimulationData PerformRunWithAdUntilSucessful(SimulationContext context)
        {
            var gatesTaken = 0;
            BigInteger reward = 0;
            do
            {
                var runthroughResult = SingleRunthrough(context);
                reward = runthroughResult.reward;
                gatesTaken += runthroughResult.gatesTaken;
            } while (reward <= 0);
            
            var secondsToFinish = gatesTaken*context.SecondsPerGate;
            
            reward = ApplyTargetMultipliers(reward, context);
            secondsToFinish += _finishingSceneSeconds;
                                   
            var ad = _adSelector.AccountForAd();
            
            var finalScore = reward * new BigInteger(ad.Multiplier);
            secondsToFinish += ad.SecondsToWatch;
            
            return new SimulationData(context.TargetScore, context.Sequence.BestPossibleResult, finalScore, secondsToFinish);
        }
        
        public (BigInteger reward, int gatesTaken) SingleRunthrough(SimulationContext context)
        {
            var runReward = context.InitialValue;
            var gateCount = 0;
            foreach(var pair in context.Sequence.Sequence)
            {
                gateCount++;
                var opertaion = _gateSelector.Choose(pair, runReward);
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
                        emptyTargets = true;
                        newReward = newReward.multiplyByFraction(target.Grade.RewardMultiplier());
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
        
        public SequenceContext BuyUpgrades()
        {            
            return new SequenceContext();
        }
        
    }
}