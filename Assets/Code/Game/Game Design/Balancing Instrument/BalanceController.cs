using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.GameDesign
{
    public class BalanceController 
    {        
        DataRetriever _dataRetriever;
        DataPlotter _dataPlotter;
        
        Texture2D _noDataTexture;
        Dictionary<GraphType, ChartDataPoint[]> _dataForCharts; 
        Dictionary<GraphType, GraphTextureContainer> _graphCache = new Dictionary<GraphType, GraphTextureContainer>();           

        public BalanceController(DataRetriever dataRetriever, DataPlotter dataPlotter)
        {
            _dataRetriever = dataRetriever ?? throw new ArgumentNullException(nameof(dataRetriever));
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            _noDataTexture = Resources.Load<Texture2D>("NoDataSplash")?? Texture2D.whiteTexture;
            _dataForCharts = Enum.GetValues(typeof(GraphType)).OfType<GraphType>()
                .ToDictionary(type => type, type => new ChartDataPoint[0]);
            
        }
        
        public async void SimulatePlaythroughs(int playthroughsToSimulate, int repeatsPerSimulator)
        {            
            var barId = Progress.Start("Simulation progress");        
            var progressIndicator = new Progress<SimProgressReport>(progress => 
                    Progress.Report(barId, progress.Part(), $"Count: {progress.FinishedCount} / {progress.CountToFinish}"));        
            var playthroughsResults = await _dataRetriever.SimulateForStatistics(playthroughsToSimulate, repeatsPerSimulator, progressIndicator);  
            Progress.Remove(barId);
            
            RenderPointsForRewardPerRun(playthroughsResults); 
            RenderPointsForUpgradesPerRun(playthroughsResults);      
            RedrawCachedGraphs();
        }       
        
        void RenderPointsForRewardPerRun(IEnumerable<PlaythroughData> simulationResults)
        {
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
                        
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
            _dataForCharts[GraphType.RewardPerRun] = dataPoints.ToArray();
        }
        
        void RenderPointsForUpgradesPerRun(IEnumerable<PlaythroughData> simulationResults)
        {            
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
                                    
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
            _dataForCharts[GraphType.UpgradesPerRun] = dataPoints.ToArray();
        }
        
        void RedrawCachedGraphs()
        {
            _dataForCharts
                .Where(kvp => _graphCache.Keys.Contains(kvp.Key))
                .Select(kvp => (data: kvp, cached: _graphCache[kvp.Key]))
                .ToList()
                .ForEach(entry => UpdateCachedGraph(entry.data.Key, entry.data.Value, entry.cached.Dimensions));
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D DrawGraph(GraphType graph, Rect textureSize)
        {
            var data = _dataForCharts[graph];
            if(data == null || !data.Any())
                return _noDataTexture;
                                            
            RenderUnrenderedImages();
            
            GraphTextureContainer cachedGraph = null;
            _graphCache.TryGetValue(graph, out cachedGraph);
            var dimensions = new Vector2Int((int)textureSize.width, (int)textureSize.height);
            if(cachedGraph != null && cachedGraph.Dimensions == dimensions)
                return cachedGraph.Texture;
                         
            UpdateCachedGraph(graph, data, dimensions);
            RenderUnrenderedImages();
            
            return _graphCache[graph].Texture;
        }
        
        void UpdateCachedGraph(GraphType graph, IEnumerable<ChartDataPoint> data, Vector2Int dimensions)
        {
            string image;
            switch(graph)
            {
                case GraphType.RewardPerRun:
                    image = _dataPlotter.PlotXYLog(data, dimensions); 
                    break;
                case GraphType.UpgradesPerRun:
                    image = _dataPlotter.PlotXY(data, dimensions); 
                    break;
                default:
                    throw new Exception("Unknows graph type");
            }
            
            while(!_graphCache.TryAdd(graph, new GraphTextureContainer(image, dimensions)))
                _graphCache.Remove(graph);
        }
        
        /// <summary> Only works on main thread </summary>
        void RenderUnrenderedImages()
        {
            _graphCache
                .Where(entry => entry.Value.Unrendered)
                .ToList()
                .ForEach(entry => entry.Value.RenderTexture());
        }
    }
}