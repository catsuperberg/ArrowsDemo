using AssetScripts.Instantiation;
using DataManagement;
using Input.ControllerComponents;
using System;
using System.Numerics;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public class ProjectileGenerator : MonoBehaviour, IProjectileProvider
    {        
        [SerializeField]
        private GameObject arrowBundle;
        
        IRegistryIngester _settingsRegistry;
        
        [Inject]
        public void Construct([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided to " + this.GetType().Name);
                
            _settingsRegistry = registry; 
        }
        
        public GameObject CreateArrows(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new System.ArgumentNullException("IInstatiator isn't provided for: " + this.GetType().Name);
                
            var bundle = assetInstatiator.Instantiate(arrowBundle, name: "Projectile (Arrow bundle)");
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript == null)
                throw new System.Exception("No IProjectileObject in selected prefab");
            bundleScript.Initialize(initialCount, movementWidth, collisionEnabled: false);
            var configurableComponent = bundle.AddComponent<TouchTranslationMovementController>(); // FIXME should search for generic IConfigurable with init\register method
            configurableComponent.Initialize(_settingsRegistry);
            return bundle;                                
        }
    }
}
