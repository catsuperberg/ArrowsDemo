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
    public class TokenDisplay : MonoBehaviour
    {        
        [SerializeField]
        private TMP_Text TokenIndicator;
        
        IRegistryValueReader _curenciesDataReader;
        
        BigInteger _localValue;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryAccessor)
        {            
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            
            _curenciesDataReader = registryAccessor;
            _curenciesDataReader.OnNewData += DataInRegistryUpdated;
            UpdateAppearanceFromRegistry();
        }
        
        void DataInRegistryUpdated(object caller, EventArgs args)
        {
            UpdateAppearanceFromRegistry();
        }
        
        public void ForceSetDisplayedAmmount(BigInteger ammountToDisplay)
        {
            _localValue = ammountToDisplay;
            UpdateAppearanceFromLocal();
        }
        
        void UpdateAppearanceFromRegistry()
        {
            var tokensString = _curenciesDataReader.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.SkinTokens));
            var ammountOfTokens = BigInteger.Parse(tokensString);
            TokenIndicator.text = ammountOfTokens.ParseToReadable();
        }      
        
        void UpdateAppearanceFromLocal()
        {
            TokenIndicator.text = _localValue.ParseToReadable();
        }     
    }
}
