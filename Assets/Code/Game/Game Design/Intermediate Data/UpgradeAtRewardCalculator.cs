using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Game.GameDesign
{        
    public class UpgradeAtRunCalculator
    {
        Dictionary<double, int> _data;    
        
        public UpgradeAtRunCalculator(IEnumerable<PlaythroughData> simulationResults)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            
            CreateDataPoints(simulationResults);            
        }
        
        public int GetUpgradeCount(double reward)
        {
            var valueBefore = _data.Keys.FirstOrDefault(value => value <= reward);
            return _data.TryGetValue(valueBefore, out var value) ? value : 0;
        }
        
        void CreateDataPoints(IEnumerable<PlaythroughData> simulationResults)
        {
            var rewardsWithUpgrades = simulationResults
                .Select(playthrough => playthrough.Runs
                    .Where(run => run.FinalScore <= playthrough.CompletionConditions.RewardLimit)
                    .Zip(playthrough.UpgradesPerRun, (run, upgrade) => new {reward = run.FinalScore, upgradeCount = upgrade}))
                .SelectMany(playthroughs => playthroughs)
                .ToList();
                
            var averagedUpgradesPerReward = rewardsWithUpgrades
                .GroupBy(entry => entry.reward)
                .Select(group => (reward: group.Key, upgradeCount: group.Select(entry => entry.upgradeCount).Average()))
                .OrderBy(entry => entry.reward)
                .Where(entry => entry.reward != System.Numerics.BigInteger.Zero)
                .ToList();                
                
            var window = 40;
            var outputSize = 500;            
            var smoothedValues = averagedUpgradesPerReward
                .MovingAverage(entry => entry.upgradeCount, (item, value) => (reward: item.reward, upgradeCount: value), window)
                .Where(entry => !Double.IsNaN(entry.upgradeCount))
                .SimplifyToSize(outputSize)
                .ToList();

                
            _data = smoothedValues
                .ToDictionary(upgradesPer => (double)upgradesPer.reward, upgradesPer => (int)Math.Round(upgradesPer.upgradeCount));
        }
    }
}