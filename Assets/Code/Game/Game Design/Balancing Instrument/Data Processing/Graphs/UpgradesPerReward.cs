using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                .Select(result => (runs: result.Runs, upgrades: result.UpgradesPerRun))
                .Select(playthrough => playthrough.runs.Zip(playthrough.upgrades, (run, upgrade) => new {reward = run.FinalScore, upgradeCount = upgrade}))
                .SelectMany(playthroughs => playthroughs)
                .ToList();
                
            var averagedUpgradesPerReward = rewardsWithUpgrades
                .GroupBy(entry => entry.reward)
                .Select(group => (result: group.Key, upgrades: group.Select(entry => entry.upgradeCount).Average()))
                .OrderBy(entry => entry.result)
                .Where(entry => entry.result != System.Numerics.BigInteger.Zero)
                .ToList();
                
            var dataPoints = averagedUpgradesPerReward.Select(upgradesPer => new ChartDataPoint((double)upgradesPer.result, upgradesPer.upgrades));
            _data = dataPoints.ToArray();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetTexture(Vector2Int dimensions)
            => GraphTexture(dimensions, () => _dataPlotter.PlotXLogY(_data, dimensions));
    }
}