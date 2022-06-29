using System;
using System.Numerics;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public class VolatileProjectileSkinData
    {
        private readonly string _name;
        private readonly UnityEngine.Object _prefabResource;
        private readonly Sprite _icon;  
        private readonly string _modelCheckSum;
        private readonly string _iconCheckSum;    
        private readonly BigInteger _baseCost;
        private readonly bool _adWatchRequired;

        public VolatileProjectileSkinData(string name, UnityEngine.Object prefabResource, Sprite icon, string modelCheckSum, string iconCheckSum, BigInteger baseCost, bool adWatchRequired)
        {
            _name = name;
            _prefabResource = prefabResource;
            _icon = icon;
            _modelCheckSum = modelCheckSum;
            _iconCheckSum = iconCheckSum;
            _baseCost = baseCost;
            _adWatchRequired = adWatchRequired;
        }
    }
}