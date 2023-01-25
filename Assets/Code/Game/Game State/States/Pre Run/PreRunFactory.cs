using DataManagement;
using Game.Gameplay.Meta;
using Game.Gameplay.Realtime;
using Game.Gameplay.Meta.Shop;
using System;
using UI;
using UnityEngine;
using Zenject;

namespace Game.GameState
{
    public class PreRunFactory
    {
        RunthroughContextManager _contextManager;  
        IUpgradeContextNotifier _upgradesNotifier; 
        ISkinContextNotifier _skinNotifier; 
        IRegistryAccessor _userContextAccessor;
        PriceCalculatorFactory _priceCalculatorFactory;
        
        public PreRunFactory(RunthroughContextManager contextManager, 
            [Inject(Id = "userContextNotifier")] IUpgradeContextNotifier upgradesNotifier,
            [Inject(Id = "userContextNotifier")] ISkinContextNotifier skinNotifier,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            PriceCalculatorFactory priceCalculatorFactory)
        {
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
            _upgradesNotifier = upgradesNotifier ?? throw new ArgumentNullException(nameof(upgradesNotifier));
            _skinNotifier = skinNotifier ?? throw new ArgumentNullException(nameof(skinNotifier));
            _userContextAccessor = registryAccessor ?? throw new ArgumentNullException(nameof(registryAccessor));
            _priceCalculatorFactory = priceCalculatorFactory ?? throw new ArgumentNullException(nameof(priceCalculatorFactory));
        }
        
        public IPreRun GetPreRun(GameObject preRunPrefab, bool skipToRun)
        {
            return skipToRun ? GetSkipingPreRun() : GetDefaultPreRun(preRunPrefab);
        }

        PreRunSkipToRun GetSkipingPreRun()
        {
            var script = new GameObject("Pre Run").AddComponent<PreRunSkipToRun>();
                script.Initialize(_contextManager);
                return script;
        }
        
        PreRun GetDefaultPreRun(GameObject preRunPrefab)
        {
            var preRunGO = GameObject.Instantiate(preRunPrefab);
            var preRun = preRunGO.GetComponent<PreRun>();
            preRun.Initialize(_upgradesNotifier, _skinNotifier, _contextManager);
            var upgradeShop = preRunGO.GetComponentInChildren<ShopManager>();
            upgradeShop.Initialize(_userContextAccessor, _upgradesNotifier, _priceCalculatorFactory);
            return preRun;
        }
    }
}