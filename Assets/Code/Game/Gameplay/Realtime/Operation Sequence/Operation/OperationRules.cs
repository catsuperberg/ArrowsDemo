using System.Numerics;
using System;
using System.Collections.Generic;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{  
    public enum BestOperation
    {
        Left,
        Right,
        Both
    }  
    
    public class OperationRules : IOperationRules
    {
        const int _minFastValue = 18; // HACK because of max subtract 10 and min devision 2, after 18 FastLeftIsBest always works
        
        // public static readonly Dictionary<(OperationInstance left, OperationInstance right), BestOperation>
        
        static OperationRules()
        {
            
        }
        
        public static readonly Dictionary<Operation, (int min, int max)> _ranges = new Dictionary<Operation, (int min, int max)>(){
                    {Operation.Add, (min: 1, max:10)},
                    {Operation.Subtract, (min: 1, max:10)},
                    {Operation.Multiply, (min: 2, max:4)},
                    {Operation.Divide, (min: 2, max:5)},
                    {Operation.Blank, (min: 0, max:0)}}; 
        
        public static readonly Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>> _delegates = new Dictionary<Operation, Func<BigInteger, BigInteger, BigInteger>>(){
                    {Operation.Add, Addition},
                    {Operation.Subtract, Subtraction},
                    {Operation.Multiply, Multiplication},
                    {Operation.Divide, Division},
                    {Operation.Blank, Keep}}; 
        
        public Func<BigInteger, BigInteger, BigInteger> GetDelegate(Operation action)
            => _delegates[action];
            
        public (int min, int max) GetRange(Operation action)
            => _ranges[action];
        
        static BigInteger Addition(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Add(initialValue, operationValue);
        static BigInteger Subtraction(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Subtract(initialValue, operationValue);
        static BigInteger Multiplication(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Multiply(initialValue, operationValue);
        static BigInteger Division(BigInteger initialValue, BigInteger operationValue)
            => BigInteger.Divide(initialValue, operationValue);
        static BigInteger Keep(BigInteger initialValue, BigInteger operationValue)
            => initialValue;
    }
}