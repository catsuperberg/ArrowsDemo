using DataManagement;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Meta.Curencies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{    
    public class UpgradeShop : MonoBehaviour
    {
        [SerializeField]
        ChangersManager ChangerManager;
        
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
            CreateUpgradeChangers();            
        }
        
        public void ResetPlayerProgress()
        {
            _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(UpgradeContext));
            _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(CurenciesContext));
            ChangerManager.refreshAllValues();
        }
        
        void CreateUpgradeChangers()
        {          
            var fieldsToControl = _userContextAccessor.GetRegisteredFields(typeof(UpgradeContext));
            fieldsToControl = fieldsToControl.Intersect(FieldsToShow).ToList();
                        
            foreach(var field in fieldsToControl)
                ChangerManager.CreateChangerForValue(_userContextAccessor, typeof(UpgradeContext), field);
        }        
    }
}