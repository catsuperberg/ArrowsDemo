using AssetScripts.AssetCreation;
using DataAccess.DiskAccess.GameFolders;
using DataManagement;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{
    public class ProjectileCollectionFactory : SkinCollectionFactory
    {             
        const string _pathToDatabase = "Assets/Prefabs/Gameplay Items/Projectiles/Resources/Projectiles.json";
        const string _skinFolderName = "Projectiles";
        const string _iconizerPrefabResourcePath = "iconizer";
        ExternalSkins _externalSkinGenerator;
        override protected Type CollectionType() =>  typeof(ProjectileSkinCollection);
           
        public ProjectileCollectionFactory(
            [Inject(Id = "userRegistryIngester")] IRegistryIngester registryInjester,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            [Inject(Id = "userRegistryManager")] IRegistryManager registryManager, IGameFolders gameFolders) : base (registryInjester, registryAccessor, registryManager)
        {            
            var prefabGenerator = new BundleProjectilePrefabGenerator();
            var iconizerPrefab = Resources.Load<GameObject>(_iconizerPrefabResourcePath);  
            var iconGeneratorGo = new GameObject();
            var iconGenerator = iconGeneratorGo.AddComponent<PrefabIconGenerator>();        
            iconGenerator.Initialize(iconizerPrefab);    
            _externalSkinGenerator = new ExternalSkins(prefabGenerator, iconGenerator, Path.Combine(gameFolders.AssetInjest, _skinFolderName));
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