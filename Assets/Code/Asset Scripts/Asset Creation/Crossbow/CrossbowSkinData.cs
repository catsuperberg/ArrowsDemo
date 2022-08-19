using System;
using System.Numerics;
using Newtonsoft.Json;

namespace AssetScripts.AssetCreation
{
    [Serializable]
    public class CrossbowSkinData : ISkinData<CrossbowSkinData>, ISkinDataEnricher<CrossbowSkinData, CrossbowInjestData>
    {
        public string Name { get => _name;}
        public string PrefabPath { get => _prefabPath;}
        public string IconPath { get => _iconPath;}
        public BigInteger? BaseCost {get => _baseCost != null ? BigInteger.Parse(_baseCost) : null;}
        public bool? AdWatchRequired {get => _adWatchRequired;}

        [JsonProperty]
        private string _name;
        [JsonProperty]
        private string _prefabPath;
        [JsonProperty]
        private string _iconPath;    
        [JsonProperty]
        private string _baseCost = "0";
        [JsonProperty]
        private bool? _adWatchRequired = false;

        public CrossbowSkinData()
        {            
        }

        public CrossbowSkinData(string name, string prefabPath, string iconPath, BigInteger? baseCost, bool? adWatchRequired)
        {
            _name = name;
            _prefabPath = prefabPath;
            _iconPath = iconPath;
            _baseCost = baseCost != null ? baseCost.ToString() : null;
            _adWatchRequired = adWatchRequired;
        }
        
        public CrossbowSkinData GetNewWithUpdatedValues(CrossbowSkinData data)
        {
            var newName = data.Name ?? Name;
            var newPrefabPath = data.PrefabPath ?? PrefabPath;
            var newIconPath = data.IconPath ?? IconPath;
            var newBaseCost = data.BaseCost ?? BaseCost;
            var newAdWatchRequired = data.AdWatchRequired ?? AdWatchRequired;
            
            return new CrossbowSkinData(newName, newPrefabPath, newIconPath, newBaseCost, newAdWatchRequired);
        }
                
        public CrossbowSkinData EnrichWithDefaultValues()
        {            
            var skinDataWithDefaults = new CrossbowSkinData();            
            return new CrossbowSkinData(Name, PrefabPath, IconPath, skinDataWithDefaults.BaseCost, skinDataWithDefaults.AdWatchRequired);
        }
        
        public CrossbowSkinData EnrichWithInjestData(CrossbowInjestData injestData)
            => new CrossbowSkinData(Name, PrefabPath, IconPath, injestData.BaseCost, injestData.AdWatchRequired);
        
        public CrossbowSkinData ToSkinData(string name, CrossbowInjestData injestData)
            => new CrossbowSkinData(name, null, null, injestData.BaseCost, injestData.AdWatchRequired);            
                    
        // public CrossbowSkinData(string name, string prefabPath, string iconPath, BigInteger? baseCost, bool? adWatchRequired) : 
        //     base(name, prefabPath, iconPath, baseCost, adWatchRequired)
        // {
            
        // }
        
        // public CrossbowSkinData GetNewWithUpdatedValues(CrossbowSkinData data)
        // {
        //     var newName = data.Name ?? Name;
        //     var newPrefabPath = data.PrefabPath ?? PrefabPath;
        //     var newIconPath = data.IconPath ?? IconPath;
        //     var newBaseCost = data.BaseCost ?? BaseCost;
        //     var newAdWatchRequired = data.AdWatchRequired ?? AdWatchRequired;
            
        //     return new CrossbowSkinData(newName, newPrefabPath, newIconPath, newBaseCost, newAdWatchRequired);
        // }
    }
}