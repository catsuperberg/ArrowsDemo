using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.GameDesign
{        
    public class UpgradesPerReward : GraphAnalizer, IGraphAnalizer
    {
        public GraphType Type {get => GraphType.UpgradesPerReward;}
        ChartDataPoint[] _data;
        DataPlotter _dataPlotter;      
        
        public UpgradesPerReward(IEnumerable<PlaythroughData> simulationResults, DataPlotter dataPlotter)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            
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

                
            var dataPoints = smoothedValues
                .Select(upgradesPer => new ChartDataPoint((double)upgradesPer.reward, upgradesPer.upgradeCount));
            _data = dataPoints.ToArray();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetTexture(Vector2Int dimensions)
            => GraphTexture(dimensions, () => _dataPlotter.PlotXLogYLog(_data, dimensions));
    }
}