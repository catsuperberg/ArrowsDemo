using AssetScripts.AssetCreation;
using DataManagement;
using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameMath;
using Newtonsoft.Json;

namespace Game.Gameplay.Meta.Skins
{
    public class ProjectileCollection : IConfigurable
    {
        [StoredField]
        public List<string> BoughtSkins {get; private set;} = new List<string>();
        [StoredField]
        public string SelectedSkin {get; private set;} = "invalidSkinName";
                
        public Dictionary<string, BigInteger> SkinNamesAndPrices 
            {get => _accessibleSkins.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Price(kvp.Key));}

        Dictionary<string, ISkinProvider> _accessibleSkins;
        List<ISkinProvider> _skinProviders;

        public event EventHandler OnUpdated;        
        
        public ProjectileCollection(IRegistryIngester registry, List<ISkinProvider> skinProviders)
        {
            if(registry == null)
                throw new ArgumentNullException("IRegistryIngester not provided or empty at: " + this.GetType().Name);
            if(skinProviders == null || !skinProviders.Any())
                throw new ArgumentNullException("List<ISkinProvider> not provided or empty at: " + this.GetType().Name);
                
            registry.Register(this, true, true);
            _skinProviders = skinProviders;            
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
                
        public GameObject GetSelectedProjectileResource()
        {
            MakeSureSelectedSkinValid();
            var skinProvider = _accessibleSkins[SelectedSkin];
            return skinProvider.LoadResource(SelectedSkin) as GameObject;
        }
        
        public Sprite GetSkinIcon(string name)
        {
            if(SkinUnaccessible(name))
                throw new ArgumentNullException("No skin with such name in ProjectileCollection: " + name);
                
            var result = _accessibleSkins[name].Icon(name);
            return result;
        }
        
        public BigInteger GetSkinPrice(string name)
        {
            if(SkinUnaccessible(name))
                throw new ArgumentNullException("No skin with such name in ProjectileCollection: " + name);
                
            return _accessibleSkins[name].Price(name);
        }
        
        void MakeSureSelectedSkinValid()
        {
            if (SelectedSkin == null || SkinUnaccessible(SelectedSkin))
                SwitchToRandomSelectableIfSelectedInvalid();
        }

        bool SkinUnaccessible(string name) => !_accessibleSkins.ContainsKey(name);

        void SwitchToFirstSelectableIfSelectedInvalid() =>
            UpdateField(nameof(SelectedSkin), _accessibleSkins.Keys.First());
        
        void SwitchToRandomSelectableIfSelectedInvalid() =>
            UpdateField(nameof(SelectedSkin), _accessibleSkins.ElementAt(GlobalRandom.RandomInt(0, _accessibleSkins.Count)).Key);
            
        public void UpdateField(string fieldName, string fieldValue)
        {            
            SetFieldValue(fieldName, fieldValue);
            
            OnUpdated?.Invoke(this, EventArgs.Empty);            
        }
        
        public void UpdateFields(List<(string fieldName, string fieldValue)> updatedValues)
        {            
            if(updatedValues.Count == 0)
                throw new System.Exception("No field data in array provided to UpdateFields function of class: " + this.GetType().Name);
            
            foreach(var fieldData in updatedValues)
                SetFieldValue(fieldData.fieldName, fieldData.fieldValue);
            
            OnUpdated?.Invoke(this, EventArgs.Empty);            
        }
        
        void SetFieldValue(string fieldName, string fieldValue)
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