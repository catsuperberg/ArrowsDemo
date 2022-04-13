using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationInstance
    {            
        public readonly Operation Type;
        public readonly int Value;
        private static Dictionary<Operation, int> OperationFrequency = new Dictionary<Operation, int>(){
            {Operation.Multiply, 25},
            {Operation.Divide, 40},
            {Operation.Add, 22},
            {Operation.Subtract, 35},
            {Operation.Blank, 35}};
        private static Dictionary<Operation, float> OperationWeights = new Dictionary<Operation, float>();
        
        static OperationInstance() // OPTIMIZATION_POINT precalculate OperationWeights according to OperationFrequency
        {
            var sumOfOccurances = OperationFrequency.Sum(x => x.Value);
            var occurrenceAccumulator = 0;
            var sortedOperationFrequency = from entry in OperationFrequency orderby entry.Value descending select entry;
            foreach(var operation in sortedOperationFrequency)
            {
                occurrenceAccumulator += operation.Value;
                var weight = (float)occurrenceAccumulator / (float)sumOfOccurances;
                OperationWeights.Add(operation.Key, weight);
            }
        }
        
        public OperationInstance(Operation type, int value)
        {
            if(Enum.IsDefined(typeof(Operation), type))
                throw new System.Exception("no valid type provided on OperationInstance creation");
            Type = type;
            Value = value;
        }
        
        ///<summary> Creates instance of random type and value </summary>
        public OperationInstance() 
        {            
            System.Random rand = new System.Random(this.GetHashCode());
            
            var operation = GetOperationWithProbabilityOptimized(0.5f, rand);
            Type = operation;
            switch (operation)
            {
                case Operation.Add:
                    Value = GetValueWithWeight(1, 10, 0.1f, rand);
                    break;
                case Operation.Subtract:
                    Value = GetValueWithWeight(1, 10, 0.5f, rand);
                    break;
                case Operation.Multiply:
                    Value = GetValueWithWeight(2, 4, 0.03f, rand);
                    break;
                case Operation.Divide:
                    Value = GetValueWithWeight(2, 5, 0.8f, rand);
                    break;
                case Operation.Blank:
                    break;
                default:
                    break;
            }
        }
        
        Operation GetOperationWithProbabilityOptimized(float coeff, Random rand)
        {    
            var randomWeight = rand.NextDouble();
            foreach(var operation in OperationWeights)
                if(randomWeight < operation.Value)
                    return operation.Key;
            throw new System.Exception("GetOperationWithProbabilityOptimized() couldn't find operation suitable for generated randomWeight");
        }
        
        // Operation GetOperationWithProbability(float coeff, Random rand)
        // {     
        //     List<Tuple<double, Operation>> probabilityToOperation = new List<Tuple<double, Operation>>();
        //     probabilityToOperation.Add(new Tuple<double, Operation>(0.20, Operation.Multiply));
        //     probabilityToOperation.Add(new Tuple<double, Operation>(0.40, Operation.Divide));
        //     probabilityToOperation.Add(new Tuple<double, Operation>(0.22, Operation.Add));
        //     probabilityToOperation.Add(new Tuple<double, Operation>(0.35, Operation.Subtract));
        //     probabilityToOperation.Add(new Tuple<double, Operation>(0.35, Operation.Blank));
            
        //     var result = -1;
        //     while(!Enum.IsDefined(typeof(Operation), result))
        //     {    
        //         foreach (var option in probabilityToOperation)
        //         {
        //             double realRoll = rand.NextDouble();
        //             if (option.Item1 > realRoll)
        //                 result = (int)option.Item2;
        //         }
        //     }
        //     return (Operation)result;
        // }
        
        int GetValueWithWeight(float min, float max, float coeff, Random rand)
        {            
            coeff = MathUtils.MathClamp(coeff, 0, 1);
            var mean = (max-min)*coeff + min;
            var stdDev = 3;
            
            double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-rand.NextDouble();
            double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            randNormal = (double)MathUtils.MathClamp(randNormal, min, max);
            
            return (int)System.Math.Round(randNormal); 
        }
    }
}
