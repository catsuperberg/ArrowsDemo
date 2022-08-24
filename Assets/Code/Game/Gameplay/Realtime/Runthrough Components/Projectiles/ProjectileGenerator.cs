using AssetScripts.Instantiation;
using DataManagement;
using Game.Gameplay.Meta.Skins;
using Input.ControllerComponents;
using System;
using System.Numerics;
using UnityEngine;
using Zenject;
using Utils;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public class ProjectileGenerator : MonoBehaviour, IProjectileProvider
    {                
        IRegistryIngester _settingsRegistry;
        ProjectileCollection _projectileCollection;
        
        [Inject]
        public void Construct([Inject(Id = "settingsIngester")] IRegistryIngester registry, ProjectileCollection projectileCollection)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided to " + this.GetType().Name);
            if(projectileCollection == null)
                throw new ArgumentNullException("ProjectileCollection not provided to " + this.GetType().Name);
                
            _settingsRegistry = registry; 
            _projectileCollection = projectileCollection;
        }
        
        public GameObject CreateSelected(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new System.ArgumentNullException("IInstatiator isn't provided for: " + this.GetType().Name);
                
            var bundle = CreateSelectedSkin(assetInstatiator);            
            
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript == null)
                throw new System.Exception("No IProjectileObject in selected prefab");
            bundleScript.Initialize(initialCount, movementWidth, collisionEnabled: false);
            var configurableComponent = bundle.AddComponent<TouchTranslationMovementController>(); // FIXME should search for generic IConfigurable with init\register method
            configurableComponent.Initialize(_settingsRegistry);
            return bundle;                                
        }
        
        public GameObject CreateSelected(BigInteger initialCount, float movementWidth)
        {
            var bundle = Instantiate(_projectileCollection.GetSelectedProjectileResource());            
            
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript == null)
                throw new System.Exception("No IProjectileObject in selected prefab");
            bundleScript.Initialize(initialCount, movementWidth, collisionEnabled: false);
            var configurableComponent = bundle.AddComponent<TouchTranslationMovementController>(); // FIXME should search for generic IConfigurable with init\register method
            configurableComponent.Initialize(_settingsRegistry);
            return bundle;                                
        }
        
        GameObject CreateSelectedSkin(IInstatiator assetInstatiator)
            => assetInstatiator.Instantiate(_projectileCollection.GetSelectedProjectileResource());
    }
}
