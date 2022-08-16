using System.Numerics;
using Newtonsoft.Json;

namespace AssetScripts.AssetCreation
{
    [System.Serializable]
    public class BasicInjestData
    {
        public BigInteger BaseCost {get => BigInteger.Parse(_baseCost);}
        public bool AdWatchRequired {get => _adWatchRequired;}
        
        [JsonProperty]
        private string _baseCost = "0";
        [JsonProperty]
        private bool _adWatchRequired = false;
        
        public BasicInjestData()
        {
        }
        // public BasicInjestData()
        // {
        //     _baseCost = "0";
        //     _adWatchRequired = false;            
        // }

        // public BasicInjestData(BigInteger baseCost, bool adWatchRequired)
        // {
        //     _baseCost = baseCost.ToString();
        //     _adWatchRequired = adWatchRequired;
        // }
    }    
}