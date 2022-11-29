using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.GameDesign
{        
    public class PlaythroughTime : IValueAnalizer
    {
        public SimValueType Type {get => SimValueType.PlaythroughTime;}
        TimeSpan _averageTimeToCompletion;         
        
        public PlaythroughTime(IEnumerable<PlaythroughData> simulationResults)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            
            _averageTimeToCompletion = TimeSpan.FromSeconds(simulationResults
                .Select(result => result.CombinedTime)
                .Select(timeToCompletion => timeToCompletion.TotalSeconds)
                .Average());
        }  
        
        public string GetValue()
            => $"{_averageTimeToCompletion:hh\\:mm\\:ss\\.fff}";
    }
}