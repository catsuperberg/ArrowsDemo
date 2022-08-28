using Game.Gameplay.Meta.PassiveIncome;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Skins;
using DataManagement;
using Zenject;

namespace Game.Gameplay.Meta
{
    public class UserContextFactory
    {
        IRegistryIngester _registryIngester;
        SkinCollection _projectileCollection;
        SkinCollection _crossbowCollection;

        public UserContextFactory(
            [Inject(Id = "userRegistryIngester")] IRegistryIngester registryIngester,
            [Inject(Id = "projectiles")]  SkinCollection projectileCollection,
            [Inject(Id = "crossbows")] SkinCollection crossbowCollection)
        {
            _registryIngester = registryIngester ?? throw new System.ArgumentNullException(nameof(registryIngester));
            _projectileCollection = projectileCollection ?? throw new System.ArgumentNullException(nameof(projectileCollection));
            _crossbowCollection = crossbowCollection ?? throw new System.ArgumentNullException(nameof(crossbowCollection));
        }

        public UserContext GetContext()
        {
            var context = new UserContext(new CurenciesContext(_registryIngester), 
                new UpgradeContext(_registryIngester), new PassiveInvomceContext(),
                _projectileCollection, _crossbowCollection);
            return context;
        }
    }
}