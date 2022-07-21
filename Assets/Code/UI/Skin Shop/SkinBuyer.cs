using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Shop;
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
        
        bool _bought;
        BigInteger _price;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryValueReader registryAccessor)
        {            
            if(registryAccessor == null)
                throw new ArgumentNullException("IRegistryValueReader not provided to " + this.GetType().Name);
            
            _curencieDataReader = registryAccessor;
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
            
            _price = skinCollection.GetSkinPrice(name);
            PriceText.text = _price.ParseToReadable();
            var icon = skinCollection.GetSkinIcon(name);
            Icon.sprite = icon;
            UpdateAppearance();
        }
        
        public void Buy()
        {
            
        }
        
        void UpdateAppearance()
        {
            var spentString = _curencieDataReader.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.LifetimeSpending)); 
            var amountSpent = BigInteger.Parse(spentString);       
            BuyButton.interactable = amountSpent >= _price ? true : false;
        }
        
    }
}