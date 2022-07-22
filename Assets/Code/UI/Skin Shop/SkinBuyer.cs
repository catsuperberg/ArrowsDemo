using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Curencies;
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
        Button BuyButton;
        [SerializeField]
        TMP_Text PriceText;
        [SerializeField]
        UnityEngine.UI.Image Icon;
        
        IRegistryValueReader _curencieDataReader;
        SkinShopService _shopService;
        
        public string SkinName {get; private set;}
        BigInteger _price;
        
        public EventHandler OnSkinBought;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryAccessor, SkinShopService shopService)
        {            
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            if(shopService == null)
                throw new ArgumentNullException("SkinShopService not provided to " + this.GetType().Name);

            _curencieDataReader = registryAccessor;
            _shopService = shopService;
            _curencieDataReader.OnUpdated += DataInRegistryUpdated;
        }
        
        void DataInRegistryUpdated(object caller, EventArgs args)
        {
            UpdateAppearance();
        }
        
        public void AttachToSkin(string name, ProjectileCollection skinCollection)
        {
            if (skinCollection is null)
                throw new ArgumentNullException(nameof(skinCollection));
            
            SkinName = name;
            _price = skinCollection.GetSkinPrice(SkinName);
            PriceText.text = _price.ParseToReadable();
            var icon = skinCollection.GetSkinIcon(SkinName);
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