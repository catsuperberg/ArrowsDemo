using AssetScripts.Instantiation;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Target
{
    public interface ITargetProvider
    {
        public Task<GameObject> GetTargetAsync(List<GameObject> targetPrefabs, 
            BigInteger targetResult, (int Min, int Max) numberOfTargetsRange, IInstatiator assetInstatiator);
        List<TargetDataOnly> GetDataOnlyTargets(BigInteger targetResult, (int Min, int Max) numberOfTargetsRange);
    }
}