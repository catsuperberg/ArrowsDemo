using AssetScripts.AssetCreation;
using DataManagement;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{
    public class CrossbowCollectionFactory : SkinCollectionFactory
    {             
        const string _pathToDatabase = "Assets/Prefabs/Gameplay Items/Crossbows/Resources/Crossbows.json";
        const string _pathToExternalSkins = "Assets/Asset Injest/Runtime Injest/Crossbows";
        const string _iconizerPrefabResourcePath = "iconizer";
        ExternalSkins _externalSkinGenerator;
           
        public CrossbowCollectionFactory(
            [Inject(Id = "userRegistryIngester")] IRegistryIngester registryInjester,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            [Inject(Id = "userRegistryManager")] IRegistryManager registryManager) : base (registryInjester, registryAccessor, registryManager)
        {            
            var prefabGenerator = new SkinPrefabGenerator();
            var iconizerPrefab = Resources.Load<GameObject>(_iconizerPrefabResourcePath);    
            var iconGeneratorGo = new GameObject();
            var iconGenerator = iconGeneratorGo.AddComponent<PrefabIconGenerator>();         
            iconGenerator.Initialize(iconizerPrefab);    
            _externalSkinGenerator = new ExternalSkins(prefabGenerator, iconGenerator, _pathToExternalSkins);
            GameObject.Destroy(iconGeneratorGo);
        }
        
        override protected ISkinProvider PermanentProvider()
        {     
            var skinDatabase = new PermanentSkinsDatabase<CrossbowSkinData>(_pathToDatabase); 
            return new AssetSkinProvider<CrossbowSkinData>(skinDatabase.Skins);
        }
        
        override protected ISkinProvider ExternalProvider()
            => new ExternalSkinProvider(_externalSkinGenerator.Skins);
        
        override protected SkinCollection CreateColletcion(IRegistryIngester registry, List<ISkinProvider> skinProviders)
            => new CrossbowSkinCollection(registry, skinProviders);
    }
}