using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Game.GameDesign
{        
    public class GateSelectorStats : IValueAnalizer
    {
        public SimValueType Type {get => SimValueType.GateSelectorStats;} 
        float _wrongGateChance;      
        
        public GateSelectorStats(IEnumerable<PlaythroughData> simulationResults)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            
            var runs = simulationResults
                .SelectMany(entry => entry.Runs);
            float rightCount = runs
                .Sum(entry => entry.GateDecisions.RightCount);
            float wrongCount = runs
                .Sum(entry => entry.GateDecisions.WrongCount);
            _wrongGateChance = wrongCount/(rightCount+wrongCount);
        }  
        
        public string GetValue()
            => $"Chance of wrong gate: {NumberFormater.RoundSmallValue(_wrongGateChance*100, 2).ToString()}%";
    }
}