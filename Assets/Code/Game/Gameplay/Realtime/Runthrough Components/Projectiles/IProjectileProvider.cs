using AssetScripts.Instantiation;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public interface IProjectileProvider
    {        
        GameObject CreateArrows(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator);
    }
}