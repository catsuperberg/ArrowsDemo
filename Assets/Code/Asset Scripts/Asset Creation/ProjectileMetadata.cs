using System.Numerics;

namespace AssetScripts.AssetCreation
{
    public class ProjectileMetadata
    {
        public BigInteger BaseCost {get {return BigInteger.Parse(_baseCost);}}
        public bool AdWatchRequired {get {return _adWatchRequired;}}
        
        [UnityEngine.SerializeField]
        private string _baseCost;
        [UnityEngine.SerializeField]
        private bool _adWatchRequired;
        
        public ProjectileMetadata()
        {
            _baseCost = "0";
            _adWatchRequired = false;            
        }

        public ProjectileMetadata(BigInteger baseCost, bool adWatchRequired)
        {
            _baseCost = baseCost.ToString();
            _adWatchRequired = adWatchRequired;
        }
    }    
}