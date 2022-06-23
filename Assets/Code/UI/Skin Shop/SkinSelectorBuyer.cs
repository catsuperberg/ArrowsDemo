using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.Curencies;
using System;
using System.Numerics;
using TMPro;
using UnityEngine;

namespace UI
{    
    public class SkinSelectorBuyer : MonoBehaviour
    {
        [SerializeField]
        TMP_Text NameText;
        [SerializeField]
        TMP_Text ValueText;
        [SerializeField]
        TMP_Text PriceText;
                
        // public void AttachToValue(IRegistryAccessor registryAccessor, Type objectClass, string fieldName)
        // {
        //     if(registryAccessor == null)
        //         throw new NullReferenceException("No IRegistryAccessor implimentation provided to: " + this.GetType().Name);
            
        //     _registryAccessor = registryAccessor;
        //     _objectClass = objectClass;
        //     _fieldName = fieldName;
            
        //     var calculatorFactory = new PriceCalculatorFactory();
        //     _priceCalculator = calculatorFactory.GetCalculatorFor(_fieldName);
        //     if(NameText != null)
        //         NameText.text = _fieldName;
        //     updateValueText();            
        // }
        
        // public void BuyItem(string increment = "1")
        // {
        //     if(EnoughFunds())
        //     {
        //         ChargePlayerCoins();
        //         _registryAccessor.ApplyOperationOnRegisteredField(_objectClass, _fieldName, OperationType.Increase, increment);
        //         updateValueText();
        //     }
        // }
        
        // bool EnoughFunds()
        // {            
        //     var coinsString = _registryAccessor.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins));
        //     var Funds = BigInteger.Parse(coinsString);
        //     return Funds >= itemPrice;
        // }
        
        // void ChargePlayerCoins()
        // {
            
        //     _registryAccessor.ApplyOperationOnRegisteredField(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins), 
        //         OperationType.Decrease, itemPrice.ToString());
        // }
        
        // public void updateValueText()
        // {
        //     ValueText.text = _registryAccessor.GetStoredValue(_objectClass, _fieldName);   
        //     PriceText.text = itemPrice.ParseToReadable();
        // }
    }
}