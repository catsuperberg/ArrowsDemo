using ExtensionMethods;
using Game.Gameplay.Meta.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{        
    public class PriceAtLevel : GraphAnalizer, IGraphAnalizer
    {
        public GraphType Type {get => GraphType.RewardPerRun;}
        Dictionary<string, IEnumerable<ChartDataPoint>> _data;
        DataPlotter _dataPlotter;  
        
        const int _maxLevel = 100;
        
        public PriceAtLevel(Dictionary<string, UpgradePriceFormula> formulas, DataPlotter dataPlotter)
        {                        
            if(formulas == null || !formulas.Any())
                throw new Exception($"Calling render before {nameof(formulas)} is generated");
            _dataPlotter = dataPlotter ?? throw new ArgumentNullException(nameof(dataPlotter));
            
            _data = formulas
                .ToDictionary(formula => formula.Key, formula => GenerateSeries(formula.Value));
                    
            // var dataPoints = averageRewards.Select((value, index) => new ChartDataPoint(index, value));
            // _data = dataPoints.ToArray();
        }
        
        IEnumerable<ChartDataPoint> GenerateSeries(UpgradePriceFormula formula)
        {
            return Enumerable.Range(1, _maxLevel)
                .Select(index => (i: index, value: formula.Evaluate(index)))
                .Where(entry => entry.value <= new System.Numerics.BigInteger(double.MaxValue)) // ignore unplottable values
                .Select(entry => new ChartDataPoint(entry.i, (double)entry.value))
                .ToList();
        }
        
        /// <summary> Only works on main thread </summary>
        public Texture2D GetTexture(Vector2Int dimensions)
            => GraphTexture(dimensions, () => _dataPlotter.PlotXYLogMultiple(_data, dimensions));
    }
}