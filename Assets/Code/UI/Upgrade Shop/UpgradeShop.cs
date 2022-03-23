using DataManagement;
using Game.Gameplay.Meta.UpgradeSystem;
using UnityEngine;

namespace UI
{    
    public class UpgradeShop : MonoBehaviour
    {
        [SerializeField]
        ChangersManager ChangerManager;
        
        IRegistryAccessor _userContextAccessor;
        
        ChangersManager _changersManager;
        
        public void Initialize(IRegistryAccessor registryAccessor)
        {
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                
            _userContextAccessor = registryAccessor;
            CreateUpgradeChangers();            
        }
        
        void CreateUpgradeChangers()
        {          
            var fieldsToControl = _userContextAccessor.GetRegisteredFields(typeof(UpgradeContext));
                        
            foreach(var field in fieldsToControl)
                ChangerManager.CreateChangerForValue(_userContextAccessor, typeof(UpgradeContext), field);
        }        
    }
}