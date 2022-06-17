using AssetScripts.Instantiation;
using DataManagement;
using Input.ControllerComponents;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Zenject;
using GameMath;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public class ProjectileGenerator : MonoBehaviour, IProjectileProvider
    {                
        IRegistryIngester _settingsRegistry;
        ProjectileDatabase _projectileDatabase;
        
        [Inject]
        public void Construct([Inject(Id = "settingsIngester")] IRegistryIngester registry, ProjectileDatabase projectileDatabase)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided to " + this.GetType().Name);
            if(projectileDatabase == null)
                throw new ArgumentNullException("ProjectileDatabase not provided to " + this.GetType().Name);
                
            _settingsRegistry = registry; 
            _projectileDatabase = projectileDatabase;
        }
        
        public GameObject CreateRandom(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new System.ArgumentNullException("IInstatiator isn't provided for: " + this.GetType().Name);
                
            var bundle = CreateRandomBundle(assetInstatiator);            
            
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript == null)
                throw new System.Exception("No IProjectileObject in selected prefab");
            bundleScript.Initialize(initialCount, movementWidth, collisionEnabled: false);
            var configurableComponent = bundle.AddComponent<TouchTranslationMovementController>(); // FIXME should search for generic IConfigurable with init\register method
            configurableComponent.Initialize(_settingsRegistry);
            return bundle;                                
        }
        
        GameObject CreateRandomBundle(IInstatiator assetInstatiator) //TEMP
        {
            var prefabs = Prefabs();
            var prefabIndex = GlobalRandom.RandomInt(0, prefabs.Count);
            var selectedPrefab = prefabs[prefabIndex];
            
            return assetInstatiator.Instantiate(selectedPrefab as GameObject, name: "Projectile (Arrow bundle)");
        }
        
        List<UnityEngine.Object> Prefabs() //TEMP
        {
            List<UnityEngine.Object> resources = new List<UnityEngine.Object>();            
            foreach(var resource in _projectileDatabase.Skins)
                resources.Add(Resources.Load(resource.PrefabPath));            
            return resources;
        } 
    }
}
