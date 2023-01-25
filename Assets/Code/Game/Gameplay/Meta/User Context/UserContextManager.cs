using DataManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Zenject;

namespace Game.Gameplay.Meta
{
    public interface IUpgradeContextNotifier
    {
        public event EventHandler OnNewRunthroughComponents; 
    }
    
    public interface ISkinContextNotifier
    {
        public event EventHandler OnSelectedProjectileSkin;    
        public event EventHandler OnSelectedCrossbowSkin;       
    }
    
    public class UserContextManager : IUpgradeContextNotifier, ISkinContextNotifier
    {
        UserContext _context;        
        IRegistryManager _registryManager;      
        IRegistryValueReader _registryReader;        
                        
        public event EventHandler OnNewRunthroughComponents; 
        public event EventHandler OnSelectedProjectileSkin; 
        public event EventHandler OnSelectedCrossbowSkin; 
        
        static readonly ReadOnlyCollection<string> _runtrhoughComponents = new ReadOnlyCollection<string>(new[] {
            nameof(UpgradeSystem.UpgradeContext.CrossbowLevel), nameof(UpgradeSystem.UpgradeContext.ArrowLevel), 
            nameof(UpgradeSystem.UpgradeContext.InitialArrowCount)});
        
        public UserContextManager([Inject(Id = "userRegistryManager")]IRegistryManager registryManager, 
            [Inject(Id = "userRegistryAccessor")]IRegistryValueReader registryReader, UserContext context)
        {                                                    
            _registryManager = registryManager ?? throw new ArgumentNullException(nameof(registryManager));
            _registryReader = registryReader ?? throw new ArgumentNullException(nameof(registryReader));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            
            registryReader.OnNewData += ProcessUpdatedData;
            _registryManager.SyncRegistryAndNonVolatile();   
            _registryManager.UpdateRegistered();       
        }      
        
        
        void ProcessUpdatedData(object caller, RegistryChangeArgs args)
        {
            if(args.ClassName ==  typeof(UpgradeSystem.UpgradeContext).FullName)
                NotifyAboutUpgrades(args.Fields);
            else if(args.ClassName ==  typeof(Skins.ProjectileSkinCollection).FullName)
                NotifyAboutProjectileSkin(args);
            else if(args.ClassName ==  typeof(Skins.CrossbowSkinCollection).FullName)
                NotifyAboutCrossbowSkin(args);
            SaveToNonVolatile();
        }
        
        void NotifyAboutUpgrades(List<string> changedFields)
        {
            if(changedFields.Intersect(_runtrhoughComponents).Any())
                OnNewRunthroughComponents?.Invoke(this, EventArgs.Empty);
        }
        
        void NotifyAboutProjectileSkin(RegistryChangeArgs changedFields)
        {
            if(changedFields.Fields.Contains(nameof(Skins.SkinCollection.SelectedSkin)) && BoughtNotEmpty(changedFields))
                OnSelectedProjectileSkin?.Invoke(this, EventArgs.Empty);
        }
        
        
        void NotifyAboutCrossbowSkin(RegistryChangeArgs changedFields)
        {
            if(changedFields.Fields.Contains(nameof(Skins.SkinCollection.SelectedSkin)) && BoughtNotEmpty(changedFields))
                OnSelectedCrossbowSkin?.Invoke(this, EventArgs.Empty);
        }
        
        bool BoughtNotEmpty(RegistryChangeArgs updated) //HACK needed to prevent resets from triggering update while new SkinCollections aren't initialized
            => updated.Fields
                .Where(entry => entry.Contains("Bought"))
                .Any(entry => !(_registryReader.GetStoredValue(Type.GetType(updated.ClassName), entry) == "[]"));
        
        void SaveToNonVolatile()
            => _registryManager.SaveRegisteredToNonVolatile();
    }
}