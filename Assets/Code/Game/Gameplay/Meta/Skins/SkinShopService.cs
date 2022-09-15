using DataManagement;
using Game.Gameplay.Meta.Curencies;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{    
    public class SkinShopService
    {
        SkinCollection _skins;
        IRegistryAccessor _userRegistry;
        string _skinPriceInTokens = "1";
        
        public SkinShopService([Inject(Id = "userRegistryAccessor")] IRegistryAccessor userRegistry, SkinCollection skinCollection)
        {
            _userRegistry = userRegistry ?? throw new ArgumentNullException(nameof(userRegistry));
            _skins = skinCollection ?? throw new ArgumentNullException(nameof(skinCollection));
        }
        
        public Dictionary<string, BigInteger> SkinsPriceTable
            => _skins.SkinNamesAndPrices;
        
        public bool EnoughtSpendingForSkin(string name)
        {
            var spentString = _userRegistry.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.LifetimeSpending)); 
            var amountSpent = BigInteger.Parse(spentString);  
            var price = _skins.GetSkinPrice(name);  
            return amountSpent >= price;
        }
        
        public bool IsSelectedSkin(string name) => _skins.SelectedSkin == name;
        public bool IsBoughtSkin(string name) => _skins.BoughtSkins.Contains(name);
            
        public List<string> BoughtSkins => _skins.BoughtSkins;
        
        public BigInteger SkinPrice(string name) => _skins.GetSkinPrice(name);
         
        public Sprite SkinIcon(string name) => _skins.GetSkinIcon(name);         
        
        public bool BuySkin(string name)
        {
            if(!EnoughtTokens())
                return false;
                
            ChargePlayerToken();
            _userRegistry.ApplyOperationOnRegisteredField(_skins.GetType(), nameof(SkinCollection.BoughtSkins),
                OperationType.Append, JsonConvert.SerializeObject(new List<string>{name}));
            return true;
        }
        
        public void SelectSkin(string name)
            => _userRegistry.ApplyOperationOnRegisteredField(_skins.GetType(), nameof(SkinCollection.SelectedSkin),
                    OperationType.Replace, name);
        
        bool EnoughtTokens()
        {
            var tokensString = _userRegistry.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.SkinTokens)); 
            var amountOfTokens = BigInteger.Parse(tokensString);  
            return amountOfTokens >= BigInteger.Parse(_skinPriceInTokens);
        }
        
        void ChargePlayerToken()
            => _userRegistry.ApplyOperationOnRegisteredField(typeof(CurenciesContext), nameof(CurenciesContext.SkinTokens), 
                OperationType.Decrease, _skinPriceInTokens);
        
    }
}