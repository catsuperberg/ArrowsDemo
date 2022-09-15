using DataManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Meta.Skins
{
    public class CrossbowSkinCollection : SkinCollection
    {
        public CrossbowSkinCollection() 
            : base()
        {
        }
        
        public CrossbowSkinCollection(IRegistryIngester registry, List<ISkinProvider> skinProviders) 
            : base(registry, skinProviders)
        {
        }
    }
    
    public class ProjectileSkinCollection : SkinCollection
    {
        public ProjectileSkinCollection() 
            : base()
        {
        }
        
        public ProjectileSkinCollection(IRegistryIngester registry, List<ISkinProvider> skinProviders) 
            : base(registry, skinProviders)
        {
        }
    }
    
    public class SkinCollection : Configurable
    {
        [StoredField]
        public List<string> BoughtSkins {get; private set;} = new List<string>();
        [StoredField]
        public string SelectedSkin {get; private set;} = "invalidSkinName";
                
        public Dictionary<string, BigInteger> SkinNamesAndPrices 
            {get => _accessibleSkins.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Price(kvp.Key));}

        Dictionary<string, ISkinProvider> _accessibleSkins;
        List<ISkinProvider> _skinProviders;     
        
        public SkinCollection()
        {            
        }
        
        public SkinCollection(IRegistryIngester registry, List<ISkinProvider> skinProviders)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided or empty at: " + this.GetType().Name);
                
            registry.Register(this, true, true);
            _skinProviders = skinProviders ?? throw new ArgumentNullException(nameof(skinProviders));            
            AssembleAccessibleSkins();
        }
        
        void AssembleAccessibleSkins()
        {
            _accessibleSkins = new Dictionary<string, ISkinProvider>();
            foreach(var provider in _skinProviders)
            {
                var skinsToAdd = provider.Names.ToDictionary(key => key, value => provider);
                foreach(var skin in skinsToAdd.Where(kvp => !_accessibleSkins.ContainsKey(kvp.Key)))
                    _accessibleSkins.Add(skin.Key, skin.Value);
            }
            if(!_accessibleSkins.Any())
                throw new Exception("No accessible skins after collection assembly");
        }
        
        void SelectFirstAccessibleIfSelectedInvalid()
        {
            if(_accessibleSkins.Keys.Contains(SelectedSkin))
                return;
            
            SelectedSkin = _accessibleSkins.Keys.First(entry => BoughtSkins.Contains(entry));
        }
                
        public GameObject GetSelectedSkinResource()
        {
            var skinProvider = _accessibleSkins[SelectedSkin];
            return skinProvider.LoadResource(SelectedSkin) as GameObject;
        }
        
        public Sprite GetSkinIcon(string name)
        {
            if(SkinUnaccessible(name))
                throw new ArgumentNullException("No skin with such name in SkinCollection: " + name);
                
            var result = _accessibleSkins[name].Icon(name);
            return result;
        }
        
        public BigInteger GetSkinPrice(string name)
        {
            if(SkinUnaccessible(name))
                throw new ArgumentNullException("No skin with such name in SkinCollection: " + name);
                
            return _accessibleSkins[name].Price(name);
        }
        
        bool SkinUnaccessible(string name) => !_accessibleSkins.ContainsKey(name);
                
        internal override void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(SelectedSkin):
                    SelectedSkin = fieldValue;
                    break;
                case nameof(BoughtSkins):
                    BoughtSkins = JsonConvert.DeserializeObject<List<string>>(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}