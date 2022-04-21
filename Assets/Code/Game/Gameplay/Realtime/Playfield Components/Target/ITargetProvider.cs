using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Target
{
    public interface ITargetProvider
    {
        public GameObject GetSuitableTarget(List<GameObject> targetPrefabs, BigInteger targetResult, (int Min, int Max) numberOfTargetsRange);
        public Task<GameObject> GetSuitableTargetAsync(List<GameObject> targetPrefabs, BigInteger targetResult, (int Min, int Max) numberOfTargetsRange);
    }
}