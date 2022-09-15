using DataManagement;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.UpgradeSystem;
using System;
using UnityEngine;
using Zenject;

namespace Settings
{    
    public class ProgressReset : MonoBehaviour, ICallable
    {              
        IRegistryAccessor _userContextAccessor;
        
        [Inject]
        public void Initialize([Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {
            _userContextAccessor = registryAccessor ?? throw new ArgumentNullException(nameof(registryAccessor)); 
        }
         
        public void Call()
            => ResetPlayerProgress();
        
        void ResetPlayerProgress()
        {
            _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(UpgradeContext));
            _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(CurenciesContext));
        }
        
        public class Factory : PlaceholderFactory<ProgressReset>
        {
        }
    }
}