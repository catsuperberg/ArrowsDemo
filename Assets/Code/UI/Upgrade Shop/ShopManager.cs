using DataManagement;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Meta.Curencies;
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
        
        IRegistryAccessor _userContextAccessor;
        
        ChangersManager _changersManager;
        
        public void Initialize(IRegistryAccessor registryAccessor)
        {
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                
            _userContextAccessor = registryAccessor;
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
            buyer.AttachToValue(_userContextAccessor, typeof(UpgradeContext), field);
            buyer.updateValueText();            
        }
    }
}