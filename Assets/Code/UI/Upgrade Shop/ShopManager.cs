using DataManagement;
using Game.Gameplay.Meta;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.UpgradeSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{    
    public class ShopManager : MonoBehaviour
    {
        [SerializeField]
        ItemBuyer CrossbowBuyer;
        [SerializeField]
        ItemBuyer ArrowsBuyer;
        [SerializeField]
        ItemBuyer ArrowCountBuyer;
        [SerializeField]
        ItemBuyer PassiveIncomeBuyer;
        
        private static List<string> FieldsToShow = new List<string>(){
            nameof(UpgradeContext.ArrowLevel),
            nameof(UpgradeContext.CrossbowLevel),
            nameof(UpgradeContext.InitialArrowCount)};
        
        IUpgradeContextNotifier _upgradesNotifier; 
        IUpgradeShopService _shopService;
                
        public void Initialize(IRegistryAccessor registryAccessor, IUpgradeContextNotifier upgradesNotifier, PriceCalculatorFactory priceCalculatorFactory)
        {
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                
            _shopService = new UpgradeShopService(registryAccessor, typeof(UpgradeContext), priceCalculatorFactory);
            InitializeBuyers();     
            _upgradesNotifier = upgradesNotifier ?? throw new System.ArgumentNullException(nameof(upgradesNotifier));
            _upgradesNotifier.OnNewRunthroughComponents += UpdateBuyersText;     
        }
        
        void UpdateBuyersText(object caller, EventArgs e)
        {            
            CrossbowBuyer.updateValueText();
            ArrowsBuyer.updateValueText();
            ArrowCountBuyer.updateValueText();
            // PassiveIncomeBuyer.updateValueText();
        }
        
        void InitializeBuyers()
        {          
            AttachBuyerToRegistry(CrossbowBuyer, nameof(UpgradeContext.CrossbowLevel));
            AttachBuyerToRegistry(ArrowsBuyer, nameof(UpgradeContext.ArrowLevel));
            AttachBuyerToRegistry(ArrowCountBuyer, nameof(UpgradeContext.InitialArrowCount));
            // AttachBuyerToRegistry(PassiveIncomeBuyer, nameof(UpgradeContext.CrossbowLevel));
        }        
        
        void AttachBuyerToRegistry(ItemBuyer buyer, string field)
        {
            buyer.AttachToValue(_shopService, field);
            buyer.updateValueText();            
        }
    }
}