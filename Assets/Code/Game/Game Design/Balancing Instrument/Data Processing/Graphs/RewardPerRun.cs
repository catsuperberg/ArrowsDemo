using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{        
    public class RewardPerRun : IGraphAnalizer
    {
        public GraphType Type {get => GraphType.RewardPerRun;}
        ChartDataPoint[] _data;
        DataPlotter _dataPlotter;  
        Texture2D _cachedTexture;      
        Vector2Int _cachedDimensions;
        
        public RewardPerRun(IEnumerable<PlaythroughData> simulationResults, DataPlotter dataPlotter)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            var maxRuns = simulationResults
                .Select(result => result.Runs.Count())
                .Max();
            var range = Enumerable.Range(0, maxRuns);
            var averageRewards = range
                .Select(runNumber => (double)BigIntCalculations.Mean(simulationResults.Select(result => result.Runs.ElementAtOrDefault(runNumber))
                    .Where(run => run != null)
                    .Select(run => run.FinalScore)))
                    .ToList(); 
            
            var dataPoints = range.Select(run => new ChartDataPoint(run, averageRewards.ElementAt(run)));
            _data = dataPoints.ToArray();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GraphTexture(Vector2Int dimensions)
        {
            if(_cachedTexture != null && dimensions == _cachedDimensions)
                return _cachedTexture;
            
            var texture = new Texture2D(1,1, TextureFormat.RGBA32, false, false);
            var base64Image = _dataPlotter.PlotXYLog(_data, dimensions); 
            texture.LoadImage(Convert.FromBase64String(base64Image));
            
            _cachedTexture = texture;
            _cachedDimensions = dimensions;
            return _cachedTexture;
        }
    }
}