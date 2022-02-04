using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfildComponents.Target
{
    public interface ITargetProvider
    {
        public GameObject GetSuitableTarget(List<GameObject> targetPrefabs, BigInteger targetResult, (int Min, int Max) numberOfTargetsRange);
    }
}