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
        IEnumerable<PlaythroughData> _playthroughsResults;
        Dictionary<GraphType, IEnumerable<ChartDataPoint>> _dataForCharts;
           

        public BalanceController(DataRetriever dataRetriever, DataPlotter dataPlotter)
        {
            _dataRetriever = dataRetriever ?? throw new ArgumentNullException(nameof(dataRetriever));
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            _noDataTexture = Resources.Load<Texture2D>("NoDataSplash")?? Texture2D.whiteTexture;
            _dataForCharts = Enum.GetValues(typeof(GraphType)).OfType<GraphType>()
                .ToDictionary(type => type, type => null as IEnumerable<ChartDataPoint>);
            
        }
        
        public async void SimulatePlaythroughs(int playersToSimulate)
        {            
            var barId = Progress.Start("Simulation progress");        
            var progressIndicator = new Progress<SimProgressReport>(progress => 
                    Progress.Report(barId, progress.Part(), $"Count: {progress.FinishedCount} / {progress.CountToFinish}"));        
            _playthroughsResults = await _dataRetriever.SimulateForStatistics(playersToSimulate, progressIndicator);  
            Progress.Remove(barId);
            
            RenderPointsForRewardPerRun(); 
            RenderPointsForUpgradesPerRun();           
        }
        
        void RenderPointsForRewardPerRun()
        {
            if(_playthroughsResults == null || !_playthroughsResults.Any())
                throw new Exception($"Calling render before {nameof(_playthroughsResults)} is generated");
                        
            var maxRuns = _playthroughsResults
                .Select(result => result.Runs.Count())
                .Max();
            var range = Enumerable.Range(0, maxRuns);
            var averageRewards = range
                .Select(runNumber => (double)BigIntCalculations.Mean(_playthroughsResults.Select(result => result.Runs.ElementAtOrDefault(runNumber))
                    .Where(run => run != null)
                    .Select(run => run.FinalScore))); 
            
            IEnumerable<ChartDataPoint> dataPoints = range.Select(run => new ChartDataPoint(run, averageRewards.ElementAt(run)));
            _dataForCharts[GraphType.RewardPerRun] = dataPoints;
        }
        
        void RenderPointsForUpgradesPerRun()
        {            
            if(_playthroughsResults == null || !_playthroughsResults.Any())
                throw new Exception($"Calling render before {nameof(_playthroughsResults)} is generated");
                        
            var maxRuns = _playthroughsResults
                .Select(result => result.Runs.Count())
                .Max();
            var range = Enumerable.Range(0, maxRuns);
            var upgradeCounts = range
                .Select(runNumber => _playthroughsResults.SelectMany(result => result.UpgradesPerRun.Select((value, index) => new {value, index})))
                .SelectMany(entry => entry)
                .GroupBy(indexedValue => indexedValue.index)
                .OrderBy(entry => entry.FirstOrDefault().index)
                .ToDictionary(entry => entry.FirstOrDefault().index, entry => entry.Select(indexed => indexed.value).Average());
                
            IEnumerable<ChartDataPoint> dataPoints = upgradeCounts.Select(runs => new ChartDataPoint(runs.Key, runs.Value));
            _dataForCharts[GraphType.UpgradesPerRun] = dataPoints;
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D DrawGraph(GraphType graph, Rect textureSize)
        {
            var data = _dataForCharts[graph];
            if(data == null || !data.Any())
                return _noDataTexture;
            
            var dimensions = new Vector2Int((int)textureSize.width, (int)textureSize.height);
            switch(graph)
            {
                case GraphType.RewardPerRun:
                    return _dataPlotter.PlotXYLog(data, dimensions); 
                case GraphType.UpgradesPerRun:
                    return _dataPlotter.PlotXY(data, dimensions); 
                default:
                    return _noDataTexture;
            }
        }
    }
}