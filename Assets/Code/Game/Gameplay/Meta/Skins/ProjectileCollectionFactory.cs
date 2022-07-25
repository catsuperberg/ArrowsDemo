using AssetScripts.AssetCreation;
using DataManagement;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using GameMath;
using Newtonsoft.Json;
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
        IRegistryIngester _registryInjester;
        IRegistryAccessor _registryAccessor;
        IRegistryManager _registryManager;

        public event EventHandler OnUpdated;
        ProjectileCollection _latestCollection;
        
        
        public ProjectileCollectionFactory([Inject(Id = "userRegistryIngester")] IRegistryIngester registryInjester,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor,
            [Inject(Id = "userRegistryManager")] IRegistryManager registryManager)
        {
            _registryInjester = registryInjester ?? throw new ArgumentNullException(nameof(registryInjester));
            _registryAccessor = registryAccessor ?? throw new ArgumentNullException(nameof(registryAccessor));
            _registryManager = registryManager ?? throw new ArgumentNullException(nameof(registryManager));
        }
        
        public ProjectileCollection GetCurrentCollection()
        {
            var providers = new List<ISkinProvider>();
            providers.Add(PermanentProvider());
            _latestCollection = new ProjectileCollection(_registryInjester, providers);
            _registryManager.OnRegisteredUpdated += PrepareCollectionAfterNonVolatileLoaded; // HACK _latestCollection ony needed so it's possible to clean up non valid stuff in non volatile storage
            return _latestCollection;
        }
        
        void PrepareCollectionAfterNonVolatileLoaded(object caller, EventArgs args)
        {
            AddFreeSkinsToBought(_latestCollection);
            MakeSureSelectedSkinValid(_latestCollection);     
            _registryManager.OnRegisteredUpdated -= PrepareCollectionAfterNonVolatileLoaded;       
        }
        
        void AddFreeSkinsToBought(ProjectileCollection collection)
        {
            var skinPriceTable = collection.SkinNamesAndPrices;
            foreach(var skin in skinPriceTable.Where(entry => entry.Value == 0))
                _registryAccessor.ApplyOperationOnRegisteredField(typeof(ProjectileCollection), nameof(ProjectileCollection.BoughtSkins),
                    OperationType.Append, JsonConvert.SerializeObject(new List<string> {skin.Key}));
        }
        
        void MakeSureSelectedSkinValid(ProjectileCollection collection)
        {
            var selected = collection.SelectedSkin;
            if(collection.BoughtSkins.Contains(selected) || collection.SkinNamesAndPrices.ContainsKey(selected))
                return;
            
            SwitchToRandomSelectable(collection);
        }
        
        void SwitchToRandomSelectable(ProjectileCollection collection) 
        {
            var skinsToChooseFrom = collection.BoughtSkins.Where(x => collection.SkinNamesAndPrices.ContainsKey(x)).ToList();
            var newSelected = skinsToChooseFrom.ElementAt(GlobalRandom.RandomInt(0, skinsToChooseFrom.Count));
            _registryAccessor.ApplyOperationOnRegisteredField(typeof(ProjectileCollection), nameof(ProjectileCollection.SelectedSkin),
                    OperationType.Replace, newSelected);
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