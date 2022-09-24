using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameDesign
{
    public class OperationProbabilitiesFactory
    {
        Dictionary<Operation, int> _defaultFrequencies = new Dictionary<Operation, int>(){
            {Operation.Multiply, 40},
            {Operation.Divide, 35},
            {Operation.Add, 27},
            {Operation.Subtract, 30},
            {Operation.Blank, 35}};
                                    
        Dictionary<Operation, int> _generatedFrequencies;
            
        const string _generatedFrequenciesName = "GeneratedOperationFrequencies";
        IGameFolders _gameFolders;

        
        [Inject]
        public void Construct(IGameFolders gameFolders)
        {
            _gameFolders = gameFolders ?? throw new ArgumentNullException(nameof(gameFolders));
            _generatedFrequencies = JsonFile.LoadFromResources<Dictionary<Operation, int>>
                (Path.Combine(_gameFolders.ResourcesGameBalance,_generatedFrequenciesName));
            Debug.LogWarning(_generatedFrequencies);
        }
                    
        public MathOperationProbabilities GetDefaultProbabilities()
            => new MathOperationProbabilities(_defaultFrequencies);
            
        public MathOperationProbabilities GetFromGeneratedJson()
            => new MathOperationProbabilities(_generatedFrequencies);
    }
}