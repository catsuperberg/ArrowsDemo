using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public interface IProjectile
    {
        public GameObject ProjectilePrefab {get;}
        public BigInteger Count {get;}
        public void Initialize(BigInteger initialCount, float movementWidth);
    }
}