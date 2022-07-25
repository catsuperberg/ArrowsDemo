using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Skins;
using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{    
    public class SkinBuyer : MonoBehaviour
    {
        [SerializeField]
        UnityEngine.UI.Image Icon;
        [SerializeField]
        Button BuyButton;
        [SerializeField]
        TMP_Text PriceText;
        
        SkinShopService _shopService;
        
        public string SkinName {get; private set;}
        BigInteger _price;
        
        public EventHandler OnSkinBought;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader userRegistry, SkinShopService shopService)
        {            
            if(userRegistry == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            if(shopService == null)
                throw new ArgumentNullException("SkinShopService not provided to " + this.GetType().Name);

            _shopService = shopService;
            userRegistry.OnUpdated += DataInRegistryUpdated;
        }
        
        void DataInRegistryUpdated(object caller, EventArgs args)
        {
            UpdateAppearance();
        }
        
        public void AttachToSkin(string name)
        {
            SkinName = name;
            _price = _shopService.SkinPrice(SkinName);
            PriceText.text = _price.ParseToReadable();
            var icon = _shopService.SkinIcon(SkinName);
            Icon.sprite = icon;
            UpdateAppearance();
        }
        
        public void Buy()
        {
            _shopService.BuySkin(SkinName);
            OnSkinBought?.Invoke(this, EventArgs.Empty);
        }
        
        void UpdateAppearance()
        {
            BuyButton.interactable = _shopService.EnoughtSpendingForSkin(SkinName);
        }        
    }
}