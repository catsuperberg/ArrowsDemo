using System;
using UnityEditor;
using UnityEngine;

namespace Game.GameDesign
{
    public class BalanceController 
    {        
        DataRetriever _dataRetriever;
        DataProcessing _dataProcessor;          

        public BalanceController(DataRetriever dataRetriever, DataProcessing dataAnalizer)
        {
            _dataRetriever = dataRetriever ?? throw new ArgumentNullException(nameof(dataRetriever));
            _dataProcessor = dataAnalizer ?? throw new ArgumentNullException(nameof(dataAnalizer));            
        }
        
        public async void SimulatePlaythroughs(int playthroughsToSimulate, int repeatsPerSimulator, CompletionConditions completionConditions)
        {            
            var barId = Progress.Start("Simulation progress");        
            var progressIndicator = new Progress<SimProgressReport>(progress => 
                    Progress.Report(barId, progress.Part(), $"Count: {progress.FinishedCount} / {progress.CountToFinish}"));        
            var playthroughsResults = await _dataRetriever.SimulateForStatistics(playthroughsToSimulate, repeatsPerSimulator, progressIndicator, completionConditions);  
            Progress.Remove(barId);
            
            _dataProcessor.AnalizeSimulationResults(playthroughsResults);   
        }     
        
        /// <summary> Only works on main thread </summary>
        public Texture2D DrawGraph(GraphType graph, Rect textureSize)
            => _dataProcessor.GetGraph(graph, new Vector2Int((int)textureSize.width, (int)textureSize.height));
            
        public string GetValue(SimValueType value)
            =>_dataProcessor.GetValue(value);
    }
}