using AssetScripts.AssetCreation;
using DataManagement;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{
    public class CrossbowCollectionFactory : SkinCollectionFactory
    {             
        const string _pathToDatabase = "Assets/Prefabs/Gameplay Items/Crossbows/Resources/Crossbows.json";
           
        public CrossbowCollectionFactory(
            [Inject(Id = "userRegistryIngester")] IRegistryIngester registryInjester,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            [Inject(Id = "userRegistryManager")] IRegistryManager registryManager) : base (registryInjester, registryAccessor, registryManager)
        {
        }
        
        override protected ISkinProvider PermanentProvider()
        {     
            var skinDatabase = new PermanentSkinsDatabase<CrossbowSkinData>(_pathToDatabase); 
            return new AssetSkinProvider<CrossbowSkinData>(skinDatabase.Skins);
        }
    }
}