using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Game.GameDesign
{        
    public class UpgradeAtRunIndexCalculator
    {
        Dictionary<int, int> _data;    
        
        public UpgradeAtRunIndexCalculator(IEnumerable<PlaythroughData> simulationResults)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            
            CreateDataPoints(simulationResults);            
        }
        
        public int GetUpgradeCount(int runIndex)
        {
            var valueBefore = _data.Keys.Reverse().FirstOrDefault(value => value <= runIndex);
            return _data.TryGetValue(valueBefore, out var value) ? value : 0;
        }
        
        void CreateDataPoints(IEnumerable<PlaythroughData> simulationResults)
        {
            var upgradesPerRun = simulationResults
                .Select(result => result.UpgradesPerRun)
                .SelectMany(perRunSequence => perRunSequence.Select((value, index) => new {value, index}))
                .ToList();
            
            var groupedUpgrades = upgradesPerRun
                .GroupBy(indexedValue => indexedValue.index)
                .ToList();
            
            
            var window = 7;
            var upgradeCounts = groupedUpgrades
                .ToDictionary(entry => entry.FirstOrDefault().index, entry => entry.Select(indexed => indexed.value).Average())                
                .OrderBy(entry => entry.Key)
                .MovingAverage(entry => entry.Value, (kvp, value) => new KeyValuePair<int, double>(kvp.Key, value), window)
                .Where(entry => !Double.IsNaN(entry.Value))
                .ToList();
                
            _data = upgradeCounts
                .ToDictionary(runs => runs.Key, runs => (int)Math.Round(runs.Value));
        }
    }
}