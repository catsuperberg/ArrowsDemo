using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Runtime.RunScene.Projectiles
{    
    public interface IProjectileProvider
    {        
        GameObject CreateArrows(BigInteger initialCount, float movementWidth);
    }
}