using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{        
    public class UpgradesPerRun : GraphAnalizer, IGraphAnalizer
    {
        public GraphType Type {get => GraphType.UpgradesPerRun;}
        ChartDataPoint[] _data;
        DataPlotter _dataPlotter;      
        
        public UpgradesPerRun(IEnumerable<PlaythroughData> simulationResults, DataPlotter dataPlotter)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            var upgradesPerRun = simulationResults
                .Select(result => result.UpgradesPerRun)
                .SelectMany(perRunSequence => perRunSequence.Select((value, index) => new {value, index}))
                .ToList();
            
            var groupedUpgrades = upgradesPerRun
                .GroupBy(indexedValue => indexedValue.index)
                .ToList();
                
            var upgradeCounts = groupedUpgrades
                .ToDictionary(entry => entry.FirstOrDefault().index, entry => entry.Select(indexed => indexed.value).Average())                
                .OrderBy(entry => entry.Key);
                
            var dataPoints = upgradeCounts.Select(runs => new ChartDataPoint(runs.Key, runs.Value));
            _data = dataPoints.ToArray();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetTexture(Vector2Int dimensions)
            => GraphTexture(dimensions, () => _dataPlotter.PlotXY(_data, dimensions));
    }
}