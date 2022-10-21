using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GameDesign
{
    public class OperationValueParametersFactory
    {       
        public static readonly IReadOnlyDictionary<Operation, (int min, int max, float coeff)> BaseValueParameters = new Dictionary<Operation, (int min, int max, float coeff)>(){
                    {Operation.Add, (1, 10, 0.5f)},
                    {Operation.Subtract, (1, 10, 0.5f)},
                    {Operation.Multiply, (2, 4, 0.5f)},
                    {Operation.Divide, (2, 5, 0.5f)},
                    {Operation.Blank, (0, 0, 0.5f)}};
                    
        public readonly IReadOnlyDictionary<Operation, (int min, int max, float coeff)> ValueParameters;
        
        
        public OperationValueParametersFactory(GameBalanceConfiguration balance)
        {            
            ValueParameters = ParametersWithValueSwing(balance.OperationValuesSwing);
        }    
               
        Dictionary<Operation, (int min, int max, float coeff)> ParametersWithValueSwing(float swing)
        {
            var newParameters = new Dictionary<Operation, (int min, int max, float coeff)>();
            foreach(var parameters in BaseValueParameters)
            {
                var coeff = parameters.Value.coeff;
                var newCoeff = parameters.Key.IsPositive() ? coeff * swing : coeff / swing;
                newCoeff = Math.Clamp(newCoeff, 0.01f, 0.95f);
                newParameters.Add(parameters.Key, (parameters.Value.min, parameters.Value.max, newCoeff));
            }
            return newParameters;
        }   
    }
}