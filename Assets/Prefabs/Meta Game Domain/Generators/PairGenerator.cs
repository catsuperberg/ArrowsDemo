using System;
using Sequence;

namespace GameMeta
{
    namespace Operation
    {
        public class PairGenerator
        {   
            OperationExecutor _exec;
            OperationGenerator _instanceGenerator;
            float _coefficient;
            
            public PairGenerator(float coeff, OperationGenerator instanceGenerator, OperationExecutor exec)
            {
                _exec = exec;
                _coefficient = coeff;
                _instanceGenerator = instanceGenerator;
            }
            
            public OperationPair Generate()
            {        
                OperationPair pair = new OperationPair(
                    GenerateInstance(_coefficient),
                    GenerateInstance(_coefficient),
                    _exec);
                return pair;
            }   
            
            private OperationInstance GenerateInstance(float coeff)
            {
                Operations generatedOperation = _instanceGenerator.GetOperationWithProbability(coeff);
                var value = _instanceGenerator.GenerateValue(generatedOperation, coeff);
                return new OperationInstance{operationType = generatedOperation, value = value};
            }
        }
    }
}