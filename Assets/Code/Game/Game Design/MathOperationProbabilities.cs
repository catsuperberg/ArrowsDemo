using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GameDesign
{
    public class MathOperationProbabilities
    {
        public IReadOnlyDictionary<Operation, float> OperationWeights {get => _operationWeights;}
        Dictionary<Operation, float> _operationWeights = new Dictionary<Operation, float>();
        
        public MathOperationProbabilities(Dictionary<Operation, int> operationFrequency)
        {
            if (operationFrequency is null)
                throw new ArgumentNullException(nameof(operationFrequency));
            CalculateOperationWeights(operationFrequency);
        }
        
        void CalculateOperationWeights(Dictionary<Operation, int> operationFrequency)
        {
            var sumOfOccurances = operationFrequency.Sum(x => x.Value);
            var occurrenceAccumulator = 0;
            var sortedOperationFrequency = from entry in operationFrequency orderby entry.Value descending select entry;
            foreach(var operation in sortedOperationFrequency)
            {
                occurrenceAccumulator += operation.Value;
                var weight = (float)occurrenceAccumulator / (float)sumOfOccurances;
                _operationWeights.Add(operation.Key, weight);
            }
        }
    }
}