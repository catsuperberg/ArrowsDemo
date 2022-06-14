using AssetScripts.Instantiation;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public interface IProjectileProvider
    {        
        GameObject CreateRandom(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator);
    }
}