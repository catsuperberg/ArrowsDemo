using System;
using System.Numerics;

namespace AssetScripts.AssetCreation
{
    [Serializable]
    public class ProjectileSkinData
    {
        public string Name { get => _name;}
        public string PrefabPath { get => _prefabPath;}
        public string IconPath { get => _iconPath;}
        public string ModelCheckSum { get => _modelCheckSum;}
        public string IconCheckSum { get => _iconCheckSum;}
        
        public BigInteger BaseCost {get => BigInteger.Parse(_baseCost);}
        public bool AdWatchRequired {get => _adWatchRequired;}

        [UnityEngine.SerializeField]
        private string _name;
        [UnityEngine.SerializeField]
        private string _prefabPath;
        [UnityEngine.SerializeField]
        private string _iconPath;
        [UnityEngine.SerializeField]
        private string _modelCheckSum;
        [UnityEngine.SerializeField]
        private string _iconCheckSum;        
        [UnityEngine.SerializeField]
        private string _baseCost;
        [UnityEngine.SerializeField]
        private bool _adWatchRequired;

        public ProjectileSkinData(string name, string prefabPath, string iconPath, string modelCheckSum, string iconCheckSum, BigInteger baseCost, bool adWatchRequired)
        {
            _name = name;
            _prefabPath = prefabPath;
            _iconPath = iconPath;
            _modelCheckSum = modelCheckSum;
            _iconCheckSum = iconCheckSum;
            _baseCost = baseCost.ToString();
            _adWatchRequired = adWatchRequired;
        }
    }
}