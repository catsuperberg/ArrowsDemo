using AssetScripts.AssetCreation;
using DataManagement;
using System.Collections.Generic;
using Zenject;
using UnityEngine;

namespace Game.Gameplay.Meta.Skins
{
    public class ProjectileCollectionFactory : SkinCollectionFactory
    {             
        const string _pathToDatabase = "Assets/Prefabs/Gameplay Items/Projectiles/Resources/Projectiles.json";
        const string _pathToExternalSkins = "Assets/Asset Injest/Runtime Injest/Projectiles";
        const string _iconizerPrefabResourcePath = "iconizer";
        ExternalSkins _externalSkinGenerator;
           
        public ProjectileCollectionFactory(
            [Inject(Id = "userRegistryIngester")] IRegistryIngester registryInjester,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            [Inject(Id = "userRegistryManager")] IRegistryManager registryManager) : base (registryInjester, registryAccessor, registryManager)
        {            
            var prefabGenerator = new BundleProjectilePrefabGenerator();
            var iconizerPrefab = Resources.Load<GameObject>(_iconizerPrefabResourcePath);  
            var iconGeneratorGo = new GameObject();
            var iconGenerator = iconGeneratorGo.AddComponent<PrefabIconGenerator>();        
            iconGenerator.Initialize(iconizerPrefab);    
            _externalSkinGenerator = new ExternalSkins(prefabGenerator, iconGenerator, _pathToExternalSkins);
            GameObject.Destroy(iconGeneratorGo);
        }
        
        override protected ISkinProvider PermanentProvider()
        {     
            var skinDatabase = new PermanentSkinsDatabase<ProjectileSkinData>(_pathToDatabase); 
            return new AssetSkinProvider<ProjectileSkinData>(skinDatabase.Skins);
        }
        
        override protected ISkinProvider ExternalProvider()
            => new ExternalSkinProvider(_externalSkinGenerator.Skins);
        
        override protected SkinCollection CreateColletcion(IRegistryIngester registry, List<ISkinProvider> skinProviders)
            => new ProjectileSkinCollection(registry, skinProviders);
    }
}