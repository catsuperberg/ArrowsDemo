using DataManagement;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.UpgradeSystem;
using System;
using System.Collections.Generic;
using System.Numerics;
using static GameMath.FibonacciUtils;

namespace Game.Gameplay.Meta.Shop  
{
    public class UpgradeShopService : IUpgradeShopService
    {
        private readonly IRegistryAccessor _registryAccessor;
        private readonly Type _upgradeContextType;
                
        public UpgradeShopService(IRegistryAccessor registryAccessor, Type upgradeContextType)
        {
            _registryAccessor = registryAccessor ?? throw new ArgumentNullException(nameof(registryAccessor));
            _upgradeContextType = upgradeContextType ?? throw new ArgumentNullException(nameof(upgradeContextType));
        }
        
        public bool EnoughFundsToUpgrade(string fieldName)
        {
            var coinsString = _registryAccessor.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins));
            var Funds = BigInteger.Parse(coinsString);
            return Funds >= UpgradePrice(fieldName);
        }
        
        public void BuyUpgrade(string fieldName, string fieldIncrement = "1")
        {
            if(EnoughFundsToUpgrade(fieldName))
            {
                ChargePlayerCoins(fieldName);
                _registryAccessor.ApplyOperationOnRegisteredField(_upgradeContextType, fieldName, OperationType.Increase, fieldIncrement);
                AddSkinTokenIfApplicable();
            }
        }
        
        void ChargePlayerCoins(string fieldName)
        {            
            _registryAccessor.ApplyOperationOnRegisteredField(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins), 
                OperationType.Decrease, UpgradePrice(fieldName).ToString());
            _registryAccessor.ApplyOperationOnRegisteredField(typeof(CurenciesContext), nameof(CurenciesContext.LifetimeSpending), 
                OperationType.Increase, UpgradePrice(fieldName).ToString());                
            
        }   
        
        void AddSkinTokenIfApplicable() 
        {
            if(IsFibonacci(CalculateBoughtUpgrades()))
                _registryAccessor.ApplyOperationOnRegisteredField(typeof(CurenciesContext), nameof(CurenciesContext.SkinTokens), 
                    OperationType.Increase, "1");
        }
        
        int CalculateBoughtUpgrades()
        {
            var upgrades = new List<string>();
            upgrades.Add(_registryAccessor.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.CrossbowLevel)));
            upgrades.Add(_registryAccessor.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.ArrowLevel)));
            upgrades.Add(_registryAccessor.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.InitialArrowCount)));
            upgrades.Add(_registryAccessor.GetStoredValue(typeof(UpgradeContext), nameof(UpgradeContext.PassiveIncome)));
            var sum = 0;
            foreach(var countString in upgrades)
                sum += Convert.ToInt32(countString);
            return sum;
        }
        
        public BigInteger GetUpgradePrice(string fieldName)
        {
            return UpgradePrice(fieldName);
        }
        
        public string GetUpgradeValue(string fieldName)
        {
            return _registryAccessor.GetStoredValue(_upgradeContextType, fieldName);
        }
        
        BigInteger UpgradePrice(string fieldName) 
        {
            var itemLevel = Convert.ToInt32(_registryAccessor.GetStoredValue(_upgradeContextType, fieldName));
            return PriceCalculatorFactory.GetCalculatorFor(fieldName).GetPrice(new PricingContext(itemLevel));
        }
    }
}