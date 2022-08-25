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
        SkinCollection _projectileCollection;
        
        [Inject]
        public void Construct([Inject(Id = "settingsIngester")] IRegistryIngester registry, [Inject(Id = "projectiles")] SkinCollection projectileCollection)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided to " + this.GetType().Name);
            if(projectileCollection == null)
                throw new ArgumentNullException("ProjectileCollection not provided to " + this.GetType().Name);
                
            _settingsRegistry = registry; 
            _projectileCollection = projectileCollection;
        }
        
        public GameObject CreateSelected(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator = null)
        {
            var bundle = (assetInstatiator == null) ? 
                Instantiate(_projectileCollection.GetSelectedSkinResource()) :
                CreateSelectedSkin(assetInstatiator);            
            
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript == null)
                throw new System.Exception("No IProjectileObject in selected prefab");
            bundleScript.Initialize(initialCount, movementWidth, collisionEnabled: false);
            var configurableComponent = bundle.AddComponent<TouchTranslationMovementController>(); // FIXME should search for generic IConfigurable with init\register method
            configurableComponent.Initialize(_settingsRegistry);
            return bundle;                                
        }
        
        GameObject CreateSelectedSkin(IInstatiator assetInstatiator)
            => assetInstatiator.Instantiate(_projectileCollection.GetSelectedSkinResource());
    }
}
