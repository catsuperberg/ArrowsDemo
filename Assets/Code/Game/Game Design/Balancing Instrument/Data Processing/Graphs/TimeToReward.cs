using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace Game.GameDesign
{        
    public class TimeToReward : IGraphAnalizer
    {
        public GraphType Type {get => GraphType.TimeToReward;}
        ColumnDataPoints _data;
        DataPlotter _dataPlotter;      
        Texture2D _cachedTexture;      
        Vector2Int _cachedDimensions;    
        
        public TimeToReward(IEnumerable<PlaythroughData> simulationResults, DataPlotter dataPlotter)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            var targetReward = BigInteger.Parse(
                "1.0e20", NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint); // TODO End conditions should be selectable from GUI
            var rewardLevels = PlaythroughData.LogarithmicRewardsList(targetReward, 10);
            var perPlaythroughTimeLists = simulationResults
                .SelectMany(result => result.TimeToRewards(rewardLevels))
                .ToList();
                
            var groupedByReward = perPlaythroughTimeLists
                .GroupBy(indexedValue => indexedValue.Key)
                .ToList();
                
            var sortedAverages = groupedByReward
                .ToDictionary(group => group.Key, group => group.Select(entry => entry.Value.TotalSeconds).Average())
                .OrderBy(entry => entry.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
            _data = new ColumnDataPoints(
                sortedAverages.Keys.Select(reward => reward.ParseToReadable()).ToArray(), sortedAverages.Values.ToArray());
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GraphTexture(Vector2Int dimensions)
        {
            if(_cachedTexture != null && dimensions == _cachedDimensions)
                return _cachedTexture;
            
            var texture = new Texture2D(1,1, TextureFormat.RGBA32, false, false);
            var base64Image = _dataPlotter.PlotColumns(_data, dimensions); 
            texture.LoadImage(Convert.FromBase64String(base64Image));
            
            _cachedTexture = texture;
            _cachedDimensions = dimensions;
            return _cachedTexture;
        }
    }
}