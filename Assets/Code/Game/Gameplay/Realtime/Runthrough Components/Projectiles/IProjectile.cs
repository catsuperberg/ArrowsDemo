using Game.Gameplay.Realtime.GeneralUseInterfaces;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public interface IProjectile : IPausable, IUpdatedNotification
    {
        public float MovementWidth {get;}
        public GameObject GameObject {get;}
        public GameObject ProjectilePrefab {get;}
        public BigInteger Count {get;}
        public bool CollisionEnabled {get;}
        public void Initialize(BigInteger initialCount, float movementWidth, bool collisionEnabled);
        public void EnableCollison();
        public void DisableCollison();
    }
}