using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Game.GameDesign
{        
    public class AdSelectorStats : IValueAnalizer
    {
        public SimValueType Type {get => SimValueType.AdSelectorStats;}   
        float _averageAdMultiplier;  
        
        public AdSelectorStats(IEnumerable<PlaythroughData> simulationResults)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            
            var runs = simulationResults
                .SelectMany(entry => entry.Runs)
                .ToList();
            var multipliers = runs
                .Select(run => run.AdMultiplier)
                .ToList();
            _averageAdMultiplier = multipliers.Average();
        }  
        
        public string GetValue()
            => $"Average ad multiplier: {NumberFormater.RoundSmallValue(_averageAdMultiplier, 2).ToString()}";
    }
}