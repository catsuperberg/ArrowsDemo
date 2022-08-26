using AssetScripts.AssetCreation;
using DataManagement;
using System.Collections.Generic;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{
    public class ProjectileCollectionFactory : SkinCollectionFactory
    {             
        const string _pathToDatabase = "Assets/Prefabs/Gameplay Items/Projectiles/Resources/Projectiles.json";
           
        public ProjectileCollectionFactory(
            [Inject(Id = "userRegistryIngester")] IRegistryIngester registryInjester,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            [Inject(Id = "userRegistryManager")] IRegistryManager registryManager) : base (registryInjester, registryAccessor, registryManager)
        {
        }
        
        override protected ISkinProvider PermanentProvider()
        {     
            var skinDatabase = new PermanentSkinsDatabase<ProjectileSkinData>(_pathToDatabase); 
            return new AssetSkinProvider<ProjectileSkinData>(skinDatabase.Skins);
        }
        
        override protected SkinCollection CreateColletcion(IRegistryIngester registry, List<ISkinProvider> skinProviders)
             => new ProjectileSkinCollection(registry, skinProviders);
    }
}