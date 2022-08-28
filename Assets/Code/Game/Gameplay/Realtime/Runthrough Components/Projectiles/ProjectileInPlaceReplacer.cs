using DataManagement;
using System;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public class ProjectileInPlaceReplacer
    {                
        IProjectileProvider _projectileGenerator;
        IRegistryValueReader _userContextRegistry;

        public ProjectileInPlaceReplacer(IProjectileProvider projectileGenerator, [Inject(Id = "userRegistryAccessor")] IRegistryValueReader userContextRegistry)
        {
            _projectileGenerator = projectileGenerator ?? throw new ArgumentNullException(nameof(projectileGenerator));
            _userContextRegistry = userContextRegistry ?? throw new ArgumentNullException(nameof(userContextRegistry));
        }
        
        public GameObject CreateNewProjectileFromPrototype(GameObject oldProjectile)
        {
            var old = oldProjectile.GetComponent<IProjectile>();
            return _projectileGenerator.CreateSelected(old.Count, old.MovementWidth);
        }
    }
}
