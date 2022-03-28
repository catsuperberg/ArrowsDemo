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
        
        IRegistryAccessor _coinDataAccessor;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {            
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryAccessor not provided to " + this.GetType().Name);
            
            _coinDataAccessor = registryAccessor;
            UpdateAppearance();
        }
        
        void UpdateAppearance()
        {
            var coinsString = _coinDataAccessor.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins));
            var ammountOfCoins = BigInteger.Parse(coinsString);
            CoinIndicator.text = ammountOfCoins.ParseToReadable();
        }
    }
}
