using System;
using System.Numerics;
using Newtonsoft.Json;

namespace AssetScripts.AssetCreation
{
    [Serializable]
    public class ProjectileSkinData : ISkinData<ProjectileSkinData>, ISkinDataEnricher<ProjectileSkinData, ProjectileInjestData>
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

        public ProjectileSkinData()
        {            
        }

        public ProjectileSkinData(string name, string prefabPath, string iconPath, BigInteger? baseCost, bool? adWatchRequired)
        {
            _name = name;
            _prefabPath = prefabPath;
            _iconPath = iconPath;
            _baseCost = baseCost != null ? baseCost.ToString() : null;
            _adWatchRequired = adWatchRequired;
        }
        
        public ProjectileSkinData GetNewWithUpdatedValues(ProjectileSkinData data)
        {
            var newName = data.Name ?? Name;
            var newPrefabPath = data.PrefabPath ?? PrefabPath;
            var newIconPath = data.IconPath ?? IconPath;
            var newBaseCost = data.BaseCost ?? BaseCost;
            var newAdWatchRequired = data.AdWatchRequired ?? AdWatchRequired;
            
            return new ProjectileSkinData(newName, newPrefabPath, newIconPath, newBaseCost, newAdWatchRequired);
        }
                
        public ProjectileSkinData EnrichWithDefaultValues()
        {            
            var skinDataWithDefaults = new ProjectileSkinData();            
            return new ProjectileSkinData(Name, PrefabPath, IconPath, skinDataWithDefaults.BaseCost, skinDataWithDefaults.AdWatchRequired);
        }
        
        public ProjectileSkinData EnrichWithInjestData(ProjectileInjestData injestData)
            => new ProjectileSkinData(Name, PrefabPath, IconPath, injestData.BaseCost, injestData.AdWatchRequired);
        
        public ProjectileSkinData ToSkinData(string name, ProjectileInjestData injestData)
            => new ProjectileSkinData(name, null, null, injestData.BaseCost, injestData.AdWatchRequired);
    }
}