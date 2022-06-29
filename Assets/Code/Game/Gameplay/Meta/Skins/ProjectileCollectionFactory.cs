using AssetScripts.AssetCreation;
using DataManagement;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{
    public class ProjectileCollectionFactory
    {        
        List<ISkinProvider> _skinProviders;
        IRegistryIngester _registry;

        public event EventHandler OnUpdated;
        
        
        public ProjectileCollectionFactory([Inject(Id = "userRegistryIngester")] IRegistryIngester registry)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided or empty at: " + this.GetType().Name);
                
            _registry = registry;
        }
        
        public ProjectileCollection GetCurrentCollection()
        {
            var providers = new List<ISkinProvider>();
            providers.Add(PermanentProvider());
            
            return new ProjectileCollection(_registry, providers);
        }
        
        ISkinProvider PermanentProvider()
        {
            var skinDatabase = new ProjectileDatabase(); 
            var skinDatabaseName = "ProjectileDatabase";
            var json = Resources.Load<TextAsset>(skinDatabaseName);
            skinDatabase = JsonUtility.FromJson<ProjectileDatabase>(json.text);      
            return new AssetSkinProvider(skinDatabase.Skins);
        }
    }
}