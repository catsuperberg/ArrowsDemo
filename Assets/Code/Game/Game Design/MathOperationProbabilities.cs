using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.Linq;
using System.Collections.Generic;
using GameMath;

namespace GameDesign
{
    public class MathOperationProbabilities
    {
        public IReadOnlyDictionary<Operation, float> OperationWeights {get => _operationWeights;}
        public IReadOnlyDictionary<float, Operation> OperationsKeyedByWeigh {get => _operationsKeyedByWeigh;}
        Dictionary<Operation, float> _operationWeights = new Dictionary<Operation, float>();
        Dictionary<float, Operation> _operationsKeyedByWeigh = new Dictionary<float, Operation>();
        
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
                _operationsKeyedByWeigh.Add(weight, operation.Key);
            }
        }
    }
}