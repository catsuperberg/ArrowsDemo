using System;
using System.Collections.Generic;
using System.Linq;
using GameMath;

namespace Game.GameDesign
{            
    public class WeightedBuyerTypeProvider
    {
        public readonly Dictionary<float, BuyerType> _typeWeights;
        
        public WeightedBuyerTypeProvider(IEnumerable<PlaythroughData> simulationResults)
        {                        
            if(simulationResults == null || !simulationResults.Any())
                throw new Exception($"Calling render before {nameof(simulationResults)} is generated");
            
            _typeWeights = CalculateChancesPerType(simulationResults);            
        }
        
        public BuyerType NextBuyerType(Random rand)
            => WeightedRandom.NextFrom(_typeWeights, rand);
        
        Dictionary<float, BuyerType> CalculateChancesPerType(IEnumerable<PlaythroughData> simulationResults)
        {
            var typeFrequencies = simulationResults
                .SelectMany(playthrough => playthrough.BuyerTypePerRun)
                .GroupBy(entry => entry)
                .ToDictionary(group => group.Count(), group => group.Key);
            return WeightedRandom.FrequencyToWeights(typeFrequencies);
        }
    }
}