using System;
using System.Collections.Generic;
using Sequence;
using Utils;

namespace GameMeta
{
    namespace Operation
    {
        public class OperationGenerator
        {    
            Random _rand = new Random(Guid.NewGuid().GetHashCode());
            
            public int GenerateValue(Operations operations, float coeff)
            {
                Random _rand = new Random();
                int initValue;
                switch (operations)
                {
                    case Operations.Add:
                        initValue = GetValueWithProbability(1, 10, 0.2f);
                        return initValue;
                    case Operations.Subtract:
                        initValue = GetValueWithProbability(1, 10, 0.5f);
                        return initValue;
                    case Operations.Multiply:
                        initValue = GetValueWithProbability(2, 5, 0.2f);
                        return initValue;
                    case Operations.Divide:
                        initValue = GetValueWithProbability(2, 5, 0.5f);
                        return initValue;         
                    case Operations.Blank:
                        return 0;
                    default:
                        return 0;
                }
            }
            
            int GetValueWithProbability(float min, float max, float coeff)
            {
                coeff = MathUtils.MathClamp(coeff, 0, 1);
                var mean = (max-min)*coeff + min;
                var stdDev = 3;
                
                double u1 = 1.0-_rand.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0-_rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
                randNormal = (double)MathUtils.MathClamp(randNormal, min, max);
                
                return (int)System.Math.Round(randNormal); 
                
                // return (float)MathUtils.RoundToHalf(randNormal);        
            }
            
            public Operations GetOperationWithProbability(float coeff)
            {
                List<Tuple<double, Operations>> probabilityToOperation = new List<Tuple<double, Operations>>();
                probabilityToOperation.Add(new Tuple<double, Operations>(0.36, Operations.Multiply));
                probabilityToOperation.Add(new Tuple<double, Operations>(0.36, Operations.Divide));
                probabilityToOperation.Add(new Tuple<double, Operations>(0.28, Operations.Add));
                probabilityToOperation.Add(new Tuple<double, Operations>(0.22, Operations.Subtract));
                probabilityToOperation.Add(new Tuple<double, Operations>(0.35, Operations.Blank));
                
                var result = -1;
                while(!Enum.IsDefined(typeof(Operations), result))
                {    
                    foreach (var option in probabilityToOperation)
                    {
                        double realRoll = _rand.NextDouble();
                        if (option.Item1 > realRoll)
                            result = (int)option.Item2;
                    }
                }
                return (Operations)result;
            }
        }
    }
}