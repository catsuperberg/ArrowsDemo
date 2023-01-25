using ExtensionMethods;
using Game.Gameplay.Meta.Shop;
using System;
using TMPro;
using UnityEngine;

namespace UI
{    
    public class ItemBuyer : MonoBehaviour
    {
        [SerializeField]
        TMP_Text NameText;
        [SerializeField]
        TMP_Text ValueText;
        [SerializeField]
        TMP_Text PriceText;
        
        IUpgradeShopService _shopService;
        string _fieldName;
        
        
        public void AttachToValue(IUpgradeShopService shopService, string fieldName)
        {            
            _shopService = shopService ?? throw new NullReferenceException("No IUpgradeShopService implimentation provided to: " + this.GetType().Name);
            _fieldName = fieldName;            
            if(NameText != null)
                NameText.text = _fieldName;
            updateValueText();            
        }
        
        public void updateValueText()
        {
            ValueText.text = _shopService.GetUpgradeValue(_fieldName);   
            PriceText.text = _shopService.GetUpgradePrice(_fieldName).ParseToReadable();
        }
        
        public void BuyItem(string increment = "1")
        {
            if(_shopService.EnoughFundsToUpgrade(_fieldName))
                _shopService.BuyUpgrade(_fieldName);
            // {
            //     updateValueText();
            // }
        }
    }
}