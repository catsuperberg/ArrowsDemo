using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{        
    public class DataProcessing
    {
        DataPlotter _dataPlotter;   
        Dictionary<GraphType, IGraphAnalizer> _graphs = new Dictionary<GraphType, IGraphAnalizer>();   
        Dictionary<GraphType, IGraphAnalizer> _oldGraphs = new Dictionary<GraphType, IGraphAnalizer>();  
        Texture2D _noDataTexture; 
        
        public DataProcessing(DataPlotter dataPlotter)
        {                        
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            _noDataTexture = Resources.Load<Texture2D>("NoDataSplash")?? Texture2D.whiteTexture; 
        }
        
        public void AnalizeSimulationResults(IEnumerable<PlaythroughData> simulationResults)
        {
            if(_graphs.Any())
                _oldGraphs = _graphs;
            _graphs = new Dictionary<GraphType, IGraphAnalizer>();
            _graphs.Add(GraphType.RewardPerRun, new RewardPerRun(simulationResults, _dataPlotter));
            _graphs.Add(GraphType.UpgradesPerRun, new UpgradesPerRun(simulationResults, _dataPlotter));
            _graphs.Add(GraphType.TimeToReward, new TimeToReward(simulationResults, _dataPlotter));            
        }
        
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetGraph(GraphType type, Vector2Int textureSize)
            => _graphs.TryGetValue(type, out var graph) ? graph.GetTexture(textureSize) : GetOldGraph(type, textureSize);
            
        Texture2D GetOldGraph(GraphType type, Vector2Int textureSize)
            => _oldGraphs.TryGetValue(type, out var graph) ? graph.GetTexture(textureSize) : _noDataTexture;
    }
}