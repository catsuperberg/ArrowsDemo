// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace Level
// {
//     public class ProceduralLevelManager
//     {
//         ITrackProvider _trackGenerator;
//         ITargerProvider _targetGenerator;
//         primaryContext _context;
        
//         public void UpdateLevel(primaryContext context)
//         {
            
//             var numberOfPrecalculatedTargets = 5;
//             PopulateTargets(numberOfPrecalculatedTargets);
//         }
        
//         void PopulateTargets(int listSize)
//         {
//             //TODO how to get length indent per upgrade
//             var context = new TrackContext(length, _context.InitialValue, _context.NumberOfGates);
//             var targetScore = _trackGenerator.GetAverageMaxScore(context);
//             _targetGenerator.GetRandomisedTarget(biome, targetScore);
//         }
//     }    
// }