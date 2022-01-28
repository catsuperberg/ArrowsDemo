using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Runtime.RunScene.Projectiles
{    
    public interface IProjectile
    {
        public GameObject ProjectilePrefab {get;}
        public BigInteger Count {get;}
        public void Initialize(BigInteger initialCount, float movementWidth);
    }
}