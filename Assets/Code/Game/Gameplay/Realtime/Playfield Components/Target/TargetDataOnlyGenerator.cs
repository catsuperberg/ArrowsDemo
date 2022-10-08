using AssetScripts.Instantiation;
using GameMath;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

using Random = System.Random;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Target
{
    public class TargetDataOnlyGenerator : ITargetProvider 
    {               
        private Random _rand = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
        
        public Task<GameObject> GetTargetAsync(List<GameObject> targetPrefabs, 
            BigInteger targetResult, (int Min, int Max) numberOfTargetsRange, IInstatiator assetInstatiator)
        {
            throw new NotImplementedException();
        }
        
        public List<TargetDataOnly> GetDataOnlyTargets(BigInteger targetResult, (int Min, int Max) numberOfTargetsRange)
        {
            var numberOfTargets = _rand.Next(numberOfTargetsRange.Min, numberOfTargetsRange.Max);
            var targetScores = RandomBigIntListWithSetSum.Generate(targetResult, numberOfTargets, spreadDeviation: (0.2f, 0.85f));  
            var targets = targetScores
                .Select(entry => new TargetDataOnly(entry, (TargetGrades)_rand.Next(0, (int)TargetGrades.ENUM_END)))
                .ToList();
            return targets;
        }
    }
}



