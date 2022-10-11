using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameDesign
{
    public class OperationProbabilitiesFactory
    {
        static readonly Dictionary<Operation, int> _defaultFrequencies = new Dictionary<Operation, int>(){
            {Operation.Multiply, 40},
            {Operation.Divide, 35},
            {Operation.Add, 27},
            {Operation.Subtract, 30},
            {Operation.Blank, 35}};
                                    
        Dictionary<Operation, int> _generatedFrequencies;
            
        const string _generatedFrequenciesName = "GeneratedOperationFrequencies";
        IGameFolders _gameFolders;
        
        
        public OperationProbabilitiesFactory(IGameFolders gameFolders)
        {            
            _gameFolders = gameFolders ?? throw new ArgumentNullException(nameof(gameFolders));
            _generatedFrequencies = JsonFile.LoadFromResources<Dictionary<Operation, int>>
                (Path.Combine(_gameFolders.ResourcesGameBalance,_generatedFrequenciesName));
        }
        
        [Inject]
        public void Construct(IGameFolders gameFolders)
        {
            _gameFolders = gameFolders ?? throw new ArgumentNullException(nameof(gameFolders));
            _generatedFrequencies = JsonFile.LoadFromResources<Dictionary<Operation, int>>
                (Path.Combine(_gameFolders.ResourcesGameBalance,_generatedFrequenciesName));
        }
        
        public Dictionary<Operation, int> GetRepeatsForCertainCount(int operationCount)
        {
            Dictionary<Operation, int> result;
            var coeff = (float)operationCount/(float)(_defaultFrequencies.Values.Sum());
            var floatRepeats = _generatedFrequencies.ToDictionary(kvp => kvp.Key, kvp => (float)(kvp.Value)*coeff);
            var intRepeats = floatRepeats.ToDictionary(kvp => kvp.Key, kvp => Math.Clamp((int)MathF.Round(kvp.Value), 1 , int.MaxValue));         
            var sortedRepeats = from entry in intRepeats orderby entry.Value descending select entry;
            var deficit = operationCount - intRepeats.Values.Sum();
            if(deficit > 0)
                result = AddToLargest(sortedRepeats, deficit);
            else if(deficit < 0)
                result = SubtractFromSmallest(sortedRepeats, deficit);
            else
                result = sortedRepeats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                
            // result.ToList().ForEach(entry => UnityEngine.Debug.Log($"Operation: {entry.Key.ToString()} Repeats: {entry.Value}"));
            return result;
        }
        
        Dictionary<Operation, int> AddToLargest(IOrderedEnumerable<KeyValuePair<Operation, int>> sortedDescending, int deficit)
        {
            var oldEntry = sortedDescending.First();
            return ChangeValueOfKey(sortedDescending, oldEntry, oldEntry.Value + deficit);
        }
        
        
        Dictionary<Operation, int> SubtractFromSmallest(IOrderedEnumerable<KeyValuePair<Operation, int>> sortedDescending, int deficit)
        {
            var oldEntry = sortedDescending.LastOrDefault(kvp => kvp.Value + deficit > 0);
            if(oldEntry.Equals(default(KeyValuePair<Operation, int>)))
                throw new Exception("Couldn't fit repeats in provided operationCount");
            return ChangeValueOfKey(sortedDescending, oldEntry, oldEntry.Value + deficit);
        }
        
        Dictionary<Operation, int> ChangeValueOfKey(
            IOrderedEnumerable<KeyValuePair<Operation, int>> sorted, 
            KeyValuePair<Operation, int> oldEntry, int newValue)        
        {
            var dict = sorted.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            dict.Remove(oldEntry.Key);
            dict.Add(oldEntry.Key, newValue);
            return dict;
        }
        
                    
        public static MathOperationProbabilities GetDefaultProbabilities()
            => new MathOperationProbabilities(_defaultFrequencies);
            
        public MathOperationProbabilities GetFromGeneratedJson()
            => new MathOperationProbabilities(_generatedFrequencies);
    }
}