using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Game.GameDesign
{
    public class OperationProbabilitiesFactory
    {
        static readonly Dictionary<Operation, int> _baseFrequencies = new Dictionary<Operation, int>(){
            {Operation.Multiply, 40},
            {Operation.Divide, 35},
            {Operation.Add, 27},
            {Operation.Subtract, 30},
            {Operation.Blank, 35}};
                        
        static int _cachedSize = 0;
        static Dictionary<Operation, int> _cachedReps;
                                    
        public IReadOnlyDictionary<Operation, int> LoadedFrequencies {get; private set;}                
        
        
        public OperationProbabilitiesFactory(GameBalanceConfiguration balance)
        {            
            LoadedFrequencies = FrequenciesWithValueSwing(balance.OperationFrequencySwing);
            _cachedSize = 0;
            _cachedReps = null;
        } 
        
        Dictionary<Operation, int> FrequenciesWithValueSwing(float swing)
        {
            var newFrequencies = new Dictionary<Operation, int>();
            foreach(var frequency in _baseFrequencies)
            {
                var repeats = frequency.Value;
                var newRepeats = frequency.Key.IsPositive() ? repeats * swing : repeats / swing;
                newFrequencies.Add(frequency.Key, (int)Math.Round(Math.Clamp(newRepeats, 1, float.MaxValue)));
            }
            return newFrequencies;
        }  
        
        public static Dictionary<Operation, int> GetRepeatsForCertainCount(IReadOnlyDictionary<Operation, int> frequencies , int operationCount)
        {
            if(operationCount == _cachedSize)
                return _cachedReps;
            
            Dictionary<Operation, int> result;
            var coeff = (float)operationCount/(float)(frequencies.Values.Sum());
            var floatRepeats = frequencies.ToDictionary(kvp => kvp.Key, kvp => (float)(kvp.Value)*coeff);
            var intRepeats = floatRepeats.ToDictionary(kvp => kvp.Key, kvp => Math.Clamp((int)MathF.Round(kvp.Value), 1 , int.MaxValue));         
            var sortedRepeats = from entry in intRepeats orderby entry.Value descending select entry;
            var deficit = operationCount - intRepeats.Values.Sum();
            if(deficit > 0)
                result = AddToLargest(sortedRepeats, deficit);
            else if(deficit < 0)
                result = SubtractFromSmallest(sortedRepeats, deficit);
            else
                result = sortedRepeats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            _cachedReps = result;
            _cachedSize = _cachedReps.Sum(kvp => kvp.Value);
            return result;
        }
        
        static Dictionary<Operation, int> AddToLargest(IOrderedEnumerable<KeyValuePair<Operation, int>> sortedDescending, int deficit)
        {
            var oldEntry = sortedDescending.First();
            return ChangeValueOfKey(sortedDescending, oldEntry, oldEntry.Value + deficit);
        }
        
        
        static Dictionary<Operation, int> SubtractFromSmallest(IOrderedEnumerable<KeyValuePair<Operation, int>> sortedDescending, int deficit)
        {
            var oldEntry = sortedDescending.LastOrDefault(kvp => kvp.Value + deficit > 0);
            if(oldEntry.Equals(default(KeyValuePair<Operation, int>)))
                throw new Exception("Couldn't fit repeats in provided operationCount");
            return ChangeValueOfKey(sortedDescending, oldEntry, oldEntry.Value + deficit);
        }
        
        static Dictionary<Operation, int> ChangeValueOfKey(
            IOrderedEnumerable<KeyValuePair<Operation, int>> sorted, 
            KeyValuePair<Operation, int> oldEntry, int newValue)        
        {
            var dict = sorted.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            dict.Remove(oldEntry.Key);
            dict.Add(oldEntry.Key, newValue);
            return dict;
        }
    }
}