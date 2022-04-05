using Game.Gameplay.Meta.Curencies;
using DataManagement;
using ExtensionMethods;
using System;
using TMPro;
using UnityEngine;
using Zenject;
using System.Numerics;

namespace UI
{
    public class CoinDisplay : MonoBehaviour
    {        
        [SerializeField]
        private TMP_Text CoinIndicator;
        
        IRegistryValueReader _coinDataReader;
        
        BigInteger _localValue;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryAccessor)
        {            
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            
            _coinDataReader = registryAccessor;
            UpdateAppearanceFromRegistry();
        }
        
        public void ForceSetDisplayedAmmount(BigInteger ammountToDisplay)
        {
            _localValue = ammountToDisplay;
            UpdateAppearanceFromLocal();
        }
        
        void UpdateAppearanceFromRegistry()
        {
            var coinsString = _coinDataReader.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins));
            var ammountOfCoins = BigInteger.Parse(coinsString);
            CoinIndicator.text = ammountOfCoins.ParseToReadable();
        }      
        
        void UpdateAppearanceFromLocal()
        {
            CoinIndicator.text = _localValue.ParseToReadable();
        }     
    }
}
