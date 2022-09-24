using GameMath;
using GameDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationFactory
    {        
        IReadOnlyDictionary<Operation, float> _operationWeights;
        System.Random _rand;
                
        public OperationFactory(MathOperationProbabilities probabilities)
        {
            _operationWeights = probabilities?.OperationWeights ?? throw new ArgumentNullException(nameof(probabilities)); 
            _rand = new System.Random(this.GetHashCode());       
        }
        
        public OperationInstance GetRandom()
        {
            var state = GetRandomState();
            return new OperationInstance(state.type, state.value);
        }            
        
        (Operation type, int value) GetRandomState()
        {                        
            var operation = GetOperationWithProbabilityOptimized(0.5f);
            int value = 0;
            switch (operation)
            {
                case Operation.Add:
                    value = GetValueWithWeight(1, 10, 0.1f);
                    break;
                case Operation.Subtract:
                    value = GetValueWithWeight(1, 10, 0.5f);
                    break;
                case Operation.Multiply:
                    value = GetValueWithWeight(2, 4, 0.03f);
                    break;
                case Operation.Divide:
                    value = GetValueWithWeight(2, 5, 0.8f);
                    break;
                case Operation.Blank:
                default:
                    break;
            }
            return (operation, value);
        }
        
        Operation GetOperationWithProbabilityOptimized(float coeff)
        {    
            var randomWeight = _rand.NextDouble();
            foreach(var operation in _operationWeights)
                if(randomWeight < operation.Value)
                    return operation.Key;
            throw new System.Exception("GetOperationWithProbabilityOptimized() couldn't find operation suitable for generated randomWeight");
        }
        
        int GetValueWithWeight(float min, float max, float coeff)
        {            
            coeff = MathUtils.MathClamp(coeff, 0, 1);
            var mean = (max-min)*coeff + min;
            var stdDev = 3;
            
            double u1 = 1.0-_rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-_rand.NextDouble();
            double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            randNormal = (double)MathUtils.MathClamp(randNormal, min, max);
            
            return (int)System.Math.Round(randNormal); 
        }
        
        
        public class Factory : PlaceholderFactory<OperationFactory>
        {
        }
    }
}
