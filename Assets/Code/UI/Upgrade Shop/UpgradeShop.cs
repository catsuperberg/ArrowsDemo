using DataManagement;
using Game.Gameplay.Meta.UpgradeSystem;
using UnityEngine;
using Zenject;

namespace UI
{    
    public class UpgradeShop : MonoBehaviour
    {
        [SerializeField]
        ChangersManager ChangerManager;
        
        IRegistryAccessor _userContextAccessor;
        
        ChangersManager _changersManager;
        
        [Inject]
        public void Construct([Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {
            if(registryAccessor == null)
                throw new System.Exception("ChangerManager isn't provided to " + this.GetType().Name);
                                
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