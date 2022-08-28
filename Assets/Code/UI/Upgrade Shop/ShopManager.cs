using DataManagement;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.UpgradeSystem;
using System.Collections.Generic;
using System.Linq;
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
        
        IUpgradeShopService _shopService;
                
        public void Initialize(IRegistryAccessor registryAccessor)
        {
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                
            _shopService = new UpgradeShopService(registryAccessor, typeof(UpgradeContext));
            InitializeBuyers();            
        }
        
        // public void ResetPlayerProgress()
        // {
        //     _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(UpgradeContext));
        //     _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(CurenciesContext));
        //     ChangerManager.refreshAllValues();
        // }
        
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