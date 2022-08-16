using DataManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Meta
{
    public interface IUpgradeContextNotifier
    {
        public event EventHandler OnNewRunthroughComponents; 
    }
    
    public interface ISkinContextNotifier
    {
        public event EventHandler OnNewSelectedSkin;         
    }
    
    public class UserContextManager : IUpgradeContextNotifier, ISkinContextNotifier
    {
        UserContext _context;        
        IRegistryManager _registryManager;      
        IRegistryValueReader _registryReader;        
                        
        public event EventHandler OnNewRunthroughComponents; 
        public event EventHandler OnNewSelectedSkin; 
        
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
            else if(args.ClassName ==  typeof(Skins.ProjectileCollection).FullName)
                NotifyAboutSkins(args.Fields);
            SaveToNonVolatile();
        }
        
        void NotifyAboutUpgrades(List<string> changedFields)
        {
            if(changedFields.Intersect(_runtrhoughComponents).Any())
                OnNewRunthroughComponents?.Invoke(this, EventArgs.Empty);
        }
        
        void NotifyAboutSkins(List<string> changedFields)
        {
            if(changedFields.Contains(nameof(Skins.ProjectileCollection.SelectedSkin)))
                OnNewSelectedSkin?.Invoke(this, EventArgs.Empty);
        }
        
        void SaveToNonVolatile()
            => _registryManager.SaveRegisteredToNonVolatile();
    }
}