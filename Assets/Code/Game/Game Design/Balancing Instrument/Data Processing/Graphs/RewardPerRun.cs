using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{        
    public class RewardPerRun : GraphAnalizer, IGraphAnalizer
    {
        public GraphType Type {get => GraphType.RewardPerRun;}
        ChartDataPoint[] _data;
        DataPlotter _dataPlotter;  
        
        public RewardPerRun(IEnumerable<PlaythroughData> simulationResults, DataPlotter dataPlotter)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            var maxRuns = simulationResults
                .Select(result => result.Runs.Count())
                .Max();
            var averageRewards = Enumerable.Range(0, maxRuns)
                .Select(runNumber => (double)BigIntCalculations.Mean(simulationResults.Select(result => result.Runs.ElementAtOrDefault(runNumber))
                    .Where(run => run != null)
                    .Select(run => run.FinalScore)))
                    .ToList(); 
                    
            var dataPoints = averageRewards.Select((value, index) => new ChartDataPoint(index, value));
            _data = dataPoints.ToArray();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetTexture(Vector2Int dimensions)
            => GraphTexture(dimensions, () => _dataPlotter.PlotXYLog(_data, dimensions));
    }
}