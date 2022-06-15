using System.Numerics;

namespace AssetScripts.AssetCreation
{
    public class ProjectileInjestData
    {
        public BigInteger BaseCost {get {return BigInteger.Parse(_baseCost);}}
        public bool AdWatchRequired {get {return _adWatchRequired;}}
        
        [UnityEngine.SerializeField]
        private string _baseCost;
        [UnityEngine.SerializeField]
        private bool _adWatchRequired;
        
        public ProjectileInjestData()
        {
            _baseCost = "0";
            _adWatchRequired = false;            
        }

        public ProjectileInjestData(BigInteger baseCost, bool adWatchRequired)
        {
            _baseCost = baseCost.ToString();
            _adWatchRequired = adWatchRequired;
        }
    }    
}