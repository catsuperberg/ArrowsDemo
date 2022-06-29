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
        ProjectileCollection _projectileCollection;

        public UserContextFactory([Inject(Id = "userRegistryIngester")] IRegistryIngester registryIngester, ProjectileCollection projectileCollection)
        {
            _registryIngester = registryIngester ?? throw new System.ArgumentNullException(nameof(registryIngester));
            _projectileCollection = projectileCollection ?? throw new System.ArgumentNullException(nameof(projectileCollection));
        }

        public UserContext GetContext()
        {
            var context = new UserContext(new CurenciesContext(_registryIngester), 
                new UpgradeContext(_registryIngester), new PassiveInvomceContext(),
                _projectileCollection);
            return context;
        }
    }
}