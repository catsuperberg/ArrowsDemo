using ExtensionMethods;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using GameMath;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public class PlayerContext
    {
        public readonly UpgradeContext Upgrades;
        public readonly SequenceContext SequenceContext;
        public readonly BigInteger CurrencieCount;

        public PlayerContext(UpgradeContext upgrades, SequenceContext sequenceContext, BigInteger currencieCount)
        {
            Upgrades = upgrades ?? throw new System.ArgumentNullException(nameof(upgrades));
            SequenceContext = sequenceContext ?? throw new System.ArgumentNullException(nameof(sequenceContext));
            CurrencieCount = currencieCount;
        }
        
        public PlayerContext(PlayerContext original, UpgradeContext upgrades = null, SequenceContext sequenceContext = null, BigInteger? currencieCount = null)
        {
            Upgrades = upgrades ?? original.Upgrades;
            SequenceContext = sequenceContext ?? original.SequenceContext;
            CurrencieCount = currencieCount ?? original.CurrencieCount;
        }
    }
    
    public class VirtualPlayer
    {        
        public PlayerContext Context {get; private set;}
        
        GateSelector _gateSelector;
        IAdSelector _adSelector;
        IUpgradeBuyer _upgradeBuyer;
        
        const float _finishingSceneSeconds = 3; // HACK copied from FinishingScene  
        const float _frameTimeSeconds = 0.016f;     
        
        public VirtualPlayer(GateSelector gateSelector, IAdSelector adSelector, IUpgradeBuyer upgradeBuyer)
        {
            _gateSelector = gateSelector ?? throw new System.ArgumentNullException(nameof(gateSelector));
            _adSelector = adSelector ?? throw new System.ArgumentNullException(nameof(adSelector));
            _upgradeBuyer = upgradeBuyer ?? throw new System.ArgumentNullException(nameof(upgradeBuyer));
            
            var upgrades = new UpgradeContext();
            var sequenceContext = SimulationSequnceContextProvider.DefaultContext;
            Context = new PlayerContext(upgrades, sequenceContext, 0);
        }
        
        public void BuyUpgrades()
        {
            var results = _upgradeBuyer.BuyAll(Context.Upgrades, Context.CurrencieCount);
            var sequenceProvider = new SimulationSequnceContextProvider(Context);
            var sequence = sequenceProvider.GetContext();
            Context = new PlayerContext(Context, upgrades: results.NewUpgrades, sequenceContext: sequence, currencieCount: results.PointsLeft);            
        }
        
        public void RecieveReward(BigInteger rewardAmount)
        {
            Context = new PlayerContext(Context, currencieCount: Context.CurrencieCount + rewardAmount);
        }
        
        public RunData PerformRunWithAdUntilSucessful(SimulationContext context)
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
            
            return new RunData(context.TargetScore, context.Sequence.BestPossibleResult, finalScore, secondsToFinish);
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
    }
}