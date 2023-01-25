using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Game.GameDesign
{        
    public class UpgradesPerRun : GraphAnalizer, IGraphAnalizer
    {
        public GraphType Type {get => GraphType.UpgradesPerRun;}
        ChartDataPoint[] _data;
        DataPlotter _dataPlotter;      
        
        public UpgradesPerRun(IEnumerable<PlaythroughData> simulationResults, DataPlotter dataPlotter)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            var upgradesPerRun = simulationResults
                .Select(result => result.UpgradesPerRun)
                .SelectMany(perRunSequence => perRunSequence.Select((value, index) => new {value, index}))
                .ToList();
            
            var groupedUpgrades = upgradesPerRun
                .GroupBy(indexedValue => indexedValue.index)
                .ToList();
            
            
            var window = 7;
            var upgradeCounts = groupedUpgrades
                .ToDictionary(entry => entry.FirstOrDefault().index, entry => entry.Select(indexed => indexed.value).Average())                
                .OrderBy(entry => entry.Key)
                .MovingAverage(entry => entry.Value, (kvp, value) => new KeyValuePair<int, double>(kvp.Key, value), window)
                .Where(entry => !Double.IsNaN(entry.Value))
                .ToList();
                
            var dataPoints = upgradeCounts.Select(runs => new ChartDataPoint(runs.Key, runs.Value));
            _data = dataPoints.ToArray();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetTexture(Vector2Int dimensions)
            => GraphTexture(dimensions, () => _dataPlotter.PlotXY(_data, dimensions));
        }
    
    //HACK can't import it from utils due to them being in another assembly
    internal static class MovingAverageExtensions
    {
        public static IEnumerable<double> MovingAverage<T>(this IEnumerable<T> inputStream, Func<T, double> selector, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(selector(item));
                yield return ma.Current;
            }
        }
        
        
        public static IEnumerable<T> MovingAverage<T>(
            this IEnumerable<T> inputStream, Func<T, double> selector,
            Func<T, double, T> recreator, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(selector(item));
                yield return recreator(item, ma.Current);
            }
        }

        public static IEnumerable<double> MovingAverage(this IEnumerable<double> inputStream, int period)
        {
            var ma = new MovingAverage(period);
            foreach (var item in inputStream)
            {
                ma.Push(item);
                yield return ma.Current;
            }
        }
    }
}