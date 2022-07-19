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
    public class CurencyDisplay : MonoBehaviour, ICurencyDisplay
    {        
        [SerializeField]
        private Curency _selectedCurency;
        [SerializeField]
        private TMP_Text _curencieIndicator;
        
        IRegistryValueReader _curencieDataReader;
        
        BigInteger _localValue;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryAccessor)
        {            
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            
            _curencieDataReader = registryAccessor;
            _curencieDataReader.OnUpdated += DataInRegistryUpdated;
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
            var curenciesString = _curencieDataReader.GetStoredValue(typeof(CurenciesContext), _selectedCurency.ToString());            
            var valueType = _curencieDataReader.GetFieldType(typeof(CurenciesContext), _selectedCurency.ToString());
            var ammountOfCurency = "";
            if(valueType == typeof(BigInteger))
                ammountOfCurency = BigInteger.Parse(curenciesString).ParseToReadable();
            else
                ammountOfCurency = curenciesString.ToString();
            _curencieIndicator.text = ammountOfCurency;
        }      
        
        void UpdateAppearanceFromLocal()
        {
            _curencieIndicator.text = _localValue.ParseToReadable();
        }     
    }
}
