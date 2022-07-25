using DataManagement;
using ExtensionMethods;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.Skins;
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
        ProjectileCollection _projectileCollection;
        IRegistryAccessor _userRegistry;
        string _skinPriceInTokens = "1";
        
        public SkinShopService([Inject(Id = "userRegistryAccessor")] IRegistryAccessor userRegistry, ProjectileCollection projectileCollection)
        {
            _userRegistry = userRegistry ?? throw new ArgumentNullException(nameof(userRegistry));
            _projectileCollection = projectileCollection ?? throw new ArgumentNullException(nameof(projectileCollection));
        }
        
        public Dictionary<string, BigInteger> SkinsPriceTable
            => _projectileCollection.SkinNamesAndPrices;
        
        public bool EnoughtSpendingForSkin(string name)
        {
            var spentString = _userRegistry.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.LifetimeSpending)); 
            var amountSpent = BigInteger.Parse(spentString);  
            var price = _projectileCollection.GetSkinPrice(name);  
            return amountSpent >= price;
        }
        
        public bool IsSelectedSkin(string name) 
        {
            return _projectileCollection.SelectedSkin == name;
        }
            
        public List<string> BoughtSkins => _projectileCollection.BoughtSkins;
        
        public BigInteger SkinPrice(string name)
            => _projectileCollection.GetSkinPrice(name);
         
        public Sprite SkinIcon(string name)
            => _projectileCollection.GetSkinIcon(name);         
        
        public void BuySkin(string name)
        {
            if(!EnoughtTokens())
                return;
                
            ChargePlayerToken();
            _userRegistry.ApplyOperationOnRegisteredField(typeof(ProjectileCollection), nameof(ProjectileCollection.BoughtSkins),
                OperationType.Append, JsonConvert.SerializeObject(new List<string>{name}));
        }
        
        public void SelectSkin(string name)
            => _userRegistry.ApplyOperationOnRegisteredField(typeof(ProjectileCollection), nameof(ProjectileCollection.SelectedSkin),
                    OperationType.Replace, name);
        
        bool EnoughtTokens()
        {
            var tokensString = _userRegistry.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.SkinTokens)); 
            var amountOfTokens = BigInteger.Parse(tokensString);  
            return amountOfTokens >= BigInteger.Parse(_skinPriceInTokens);
        }
        
        void ChargePlayerToken()
        {            
            _userRegistry.ApplyOperationOnRegisteredField(typeof(CurenciesContext), nameof(CurenciesContext.SkinTokens), 
                OperationType.Decrease, _skinPriceInTokens);
        }
        
    }
}