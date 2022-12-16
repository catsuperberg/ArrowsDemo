using Game.Gameplay.Meta.Shop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.GameDesign
{
    public class BalanceController 
    {        
        DataRetriever _dataRetriever;
        DataProcessing _dataProcessor;   
        PriceCalculatorFactory _priceCalculatorFactory;       

        public BalanceController(DataRetriever dataRetriever, DataProcessing dataAnalizer, PriceCalculatorFactory priceCalculatorFactory)
        {
            _dataRetriever = dataRetriever ?? throw new ArgumentNullException(nameof(dataRetriever));
            _dataProcessor = dataAnalizer ?? throw new ArgumentNullException(nameof(dataAnalizer));
            _priceCalculatorFactory = priceCalculatorFactory ?? throw new ArgumentNullException(nameof(priceCalculatorFactory));
        }
        
        public async void SimulatePlaythroughs(int playthroughsToSimulate, int repeatsPerSimulator, CompletionConditions completionConditions)
        {                  
            var playthroughsResults = await SimulatePlaythroughsWithProgressBar(
                (progressIndicator) => _dataRetriever.SimulateForStatistics(playthroughsToSimulate, repeatsPerSimulator, progressIndicator, completionConditions));
            
            _dataProcessor.AnalizeSimulationResults(playthroughsResults);   
        }    
                
        public async void SimulateAveragePlayer(int playthroughsToSimulate, CompletionConditions completionConditions)
        {            
            if(_dataProcessor.AveragePlayer == null)
                return;
            
            var playthroughsResults = await SimulatePlaythroughsWithProgressBar(
                (progressIndicator) => _dataRetriever.SimulateAverage(playthroughsToSimulate, 1, progressIndicator, completionConditions, _dataProcessor.AveragePlayer));
            
            _dataProcessor.AnalizeAveragePlayer(playthroughsResults);   
        }      
        
        public async void GeneratePriceGraphs(GameBalanceConfiguration balance)
        {   
            var pricing = new PriceCalculatorFactory(balance);
            _dataProcessor.AnalizePricing(pricing.UpgradePriceFormulas);  
        }  
        
        async Task<IEnumerable<PlaythroughData>> SimulatePlaythroughsWithProgressBar(
            Func<Progress<SimProgressReport>, Task<IEnumerable<PlaythroughData>>> simulate)
        {
            var barId = Progress.Start("Simulation progress");        
            var progressIndicator = new Progress<SimProgressReport>(progress => 
                    Progress.Report(barId, progress.Part(), $"Count: {progress.FinishedCount} / {progress.CountToFinish}"));        
            var playthroughsResults = await simulate(progressIndicator);  
            Progress.Remove(barId);
            
            return playthroughsResults;
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D DrawGraph(GraphType graph, Rect textureSize)
            => _dataProcessor.GetGraph(graph, new Vector2Int((int)textureSize.width, (int)textureSize.height));
            
        public string GetValue(SimValueType value)
            =>_dataProcessor.GetValue(value);
    }
}