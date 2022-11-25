using System;
using UnityEditor;
using UnityEngine;

namespace Game.GameDesign
{
    public class BalanceController 
    {        
        DataRetriever _dataRetriever;
        DataProcessing _dataAnalizer;          

        public BalanceController(DataRetriever dataRetriever, DataProcessing dataAnalizer)
        {
            _dataRetriever = dataRetriever ?? throw new ArgumentNullException(nameof(dataRetriever));
            _dataAnalizer = dataAnalizer ?? throw new ArgumentNullException(nameof(dataAnalizer));
            
        }
        
        public async void SimulatePlaythroughs(int playthroughsToSimulate, int repeatsPerSimulator)
        {            
            var barId = Progress.Start("Simulation progress");        
            var progressIndicator = new Progress<SimProgressReport>(progress => 
                    Progress.Report(barId, progress.Part(), $"Count: {progress.FinishedCount} / {progress.CountToFinish}"));        
            var playthroughsResults = await _dataRetriever.SimulateForStatistics(playthroughsToSimulate, repeatsPerSimulator, progressIndicator);  
            Progress.Remove(barId);
            
            _dataAnalizer.AnalizeSimulationResults(playthroughsResults);   
        }     
        
        /// <summary> Only works on main thread </summary>
        public Texture2D DrawGraph(GraphType graph, Rect textureSize)
            => _dataAnalizer.GetGraph(graph, new Vector2Int((int)textureSize.width, (int)textureSize.height));        
    }
}