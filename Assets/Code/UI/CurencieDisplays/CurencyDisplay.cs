using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Curencies;
using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using Zenject;

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
            _curencieDataReader.OnNewData += DataInRegistryUpdated;
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
            var curencyString = _curencieDataReader.GetStoredValue(typeof(CurenciesContext), _selectedCurency.ToString());            
            var valueType = _curencieDataReader.GetFieldType(typeof(CurenciesContext), _selectedCurency.ToString());
            var ammountOfCurency = "";
            if(valueType == typeof(BigInteger))
                ammountOfCurency = BigInteger.Parse(curencyString).ParseToReadable();
            else
                ammountOfCurency = curencyString.ToString();
            _curencieIndicator.text = ammountOfCurency;
        }      
        
        void UpdateAppearanceFromLocal()
        {
            _curencieIndicator.text = _localValue.ParseToReadable();
        }     
    }
}
