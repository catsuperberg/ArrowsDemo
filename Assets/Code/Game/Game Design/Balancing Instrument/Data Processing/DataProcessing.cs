using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.GameDesign
{

    public class DataProcessing
    {
        DataPlotter _dataPlotter;   
        KeeperDictionary<GraphType, IGraphAnalizer> _graphs;   
        KeeperDictionary<SimValueType, IValueAnalizer> _values;    
        Texture2D _noDataTexture; 
        
        public DataProcessing(DataPlotter dataPlotter)
        {                        
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            _noDataTexture = Resources.Load<Texture2D>("NoDataSplash")?? Texture2D.whiteTexture;
            
            _graphs = new KeeperDictionary<GraphType, IGraphAnalizer>(null); 
            _values = new KeeperDictionary<SimValueType, IValueAnalizer>(null);
        }
        
        public void AnalizeSimulationResults(IEnumerable<PlaythroughData> simulationResults)
        {
            _graphs.ClearCurrent();
            _values.ClearCurrent();
            
            _graphs.Add(GraphType.RewardPerRun, new RewardPerRun(simulationResults, _dataPlotter));
            _graphs.Add(GraphType.UpgradesPerRun, new UpgradesPerRun(simulationResults, _dataPlotter));
            _graphs.Add(GraphType.UpgradesPerReward, new UpgradesPerReward(simulationResults, _dataPlotter));
            _graphs.Add(GraphType.TimeToReward, new TimeToReward(simulationResults, _dataPlotter));  
            
            _values.Add(SimValueType.PlaythroughTime, new PlaythroughTime(simulationResults));
            _values.Add(SimValueType.GateSelectorStats, new GateSelectorStats(simulationResults));
            _values.Add(SimValueType.AdSelectorStats, new AdSelectorStats(simulationResults));            
        }
        
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetGraph(GraphType type, Vector2Int textureSize)
            => _graphs.GetValueOrDefault(type)?.GetTexture(textureSize) ?? _noDataTexture;
        
        public string GetValue(SimValueType type)
            => _values.GetValueOrDefault(type)?.GetValue() ?? "[no data]";
    }
}