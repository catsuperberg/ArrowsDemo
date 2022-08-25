using DataManagement;
using GameMath;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Gameplay.Meta.Skins
{
    public abstract class SkinCollectionFactory
    {        
        protected List<ISkinProvider> _skinProviders;
        protected IRegistryIngester _registryInjester;
        protected IRegistryAccessor _registryAccessor;
        protected IRegistryManager _registryManager;
        
        public event EventHandler OnUpdated;
        protected SkinCollection _latestCollection;
        
        
        public SkinCollectionFactory(
            IRegistryIngester registryInjester,
            IRegistryAccessor registryAccessor,
            IRegistryManager registryManager)
        {
            _registryInjester = registryInjester ?? throw new ArgumentNullException(nameof(registryInjester));
            _registryAccessor = registryAccessor ?? throw new ArgumentNullException(nameof(registryAccessor));
            _registryManager = registryManager ?? throw new ArgumentNullException(nameof(registryManager));
        }
        
        public SkinCollection GetCurrentCollection()
        {
            var providers = new List<ISkinProvider>();
            providers.Add(PermanentProvider());
            _latestCollection = new SkinCollection(_registryInjester, providers);
            _registryManager.OnRegisteredUpdated += PrepareCollectionAfterNonVolatileLoaded; // HACK _latestCollection ony needed so it's possible to clean up non valid stuff in non volatile storage
            return _latestCollection;
        }
        
        void PrepareCollectionAfterNonVolatileLoaded(object caller, EventArgs args)
        {
            AddFreeSkinsToBought(_latestCollection);
            MakeSureSelectedSkinValid(_latestCollection);     
            _registryManager.OnRegisteredUpdated -= PrepareCollectionAfterNonVolatileLoaded;       
        }
        
        void AddFreeSkinsToBought(SkinCollection collection)
        {
            var skinPriceTable = collection.SkinNamesAndPrices;
            foreach(var skin in skinPriceTable.Where(entry => entry.Value == 0))
                _registryAccessor.ApplyOperationOnRegisteredField(typeof(SkinCollection), nameof(SkinCollection.BoughtSkins),
                    OperationType.Append, JsonConvert.SerializeObject(new List<string> {skin.Key}));
        }
        
        void MakeSureSelectedSkinValid(SkinCollection collection)
        {
            var selected = collection.SelectedSkin;
            if(collection.BoughtSkins.Contains(selected) || collection.SkinNamesAndPrices.ContainsKey(selected))
                return;
            
            SwitchToRandomSelectable(collection);
        }
        
        void SwitchToRandomSelectable(SkinCollection collection) 
        {
            var skinsToChooseFrom = collection.BoughtSkins.Where(x => collection.SkinNamesAndPrices.ContainsKey(x)).ToList();
            var newSelected = skinsToChooseFrom.ElementAt(GlobalRandom.RandomInt(0, skinsToChooseFrom.Count));
            _registryAccessor.ApplyOperationOnRegisteredField(typeof(SkinCollection), nameof(SkinCollection.SelectedSkin),
                    OperationType.Replace, newSelected);
        }
        
        protected virtual ISkinProvider PermanentProvider() => throw new NotImplementedException();
    }
}