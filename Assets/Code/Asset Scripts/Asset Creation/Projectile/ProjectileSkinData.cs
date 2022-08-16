using System;
using System.Numerics;
using Newtonsoft.Json;

namespace AssetScripts.AssetCreation
{
    [Serializable]
    public class ProjectileSkinData : ISkinData<ProjectileSkinData>
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
        
        // public ProjectileSkinData(string name, string prefabPath, string iconPath, BigInteger? baseCost, bool? adWatchRequired) : 
        //     base(name, prefabPath, iconPath, baseCost, adWatchRequired)
        // {
            
        // }
        
        // public string Name { get => _name;}
        // public string PrefabPath { get => _prefabPath;}
        // public string IconPath { get => _iconPath;}
        // public string ModelCheckSum { get => _modelCheckSum;}
        // public string IconCheckSum { get => _iconCheckSum;}
        
        // public BigInteger BaseCost {get => BigInteger.Parse(_baseCost);}
        // public bool AdWatchRequired {get => _adWatchRequired;}

        // [UnityEngine.SerializeField]
        // private string _name;
        // [UnityEngine.SerializeField]
        // private string _prefabPath;
        // [UnityEngine.SerializeField]
        // private string _iconPath;
        // [UnityEngine.SerializeField]
        // private string _modelCheckSum;
        // [UnityEngine.SerializeField]
        // private string _iconCheckSum;        
        // [UnityEngine.SerializeField]
        // private string _baseCost;
        // [UnityEngine.SerializeField]
        // private bool _adWatchRequired;

        // public ProjectileSkinData(string name, string prefabPath, string iconPath, string modelCheckSum, string iconCheckSum, BigInteger baseCost, bool adWatchRequired)
        // {
        //     _name = name;
        //     _prefabPath = prefabPath;
        //     _iconPath = iconPath;
        //     _modelCheckSum = modelCheckSum;
        //     _iconCheckSum = iconCheckSum;
        //     _baseCost = baseCost.ToString();
        //     _adWatchRequired = adWatchRequired;
        // }
    }
}