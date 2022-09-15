using DataManagement;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Meta.Skins;
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
            // can't be used as context manager emideately tries to set new skin, while not being selected
            _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(CrossbowSkinCollection)); 
            _userContextAccessor.ResetRegisteredFieldsToDefault(typeof(ProjectileSkinCollection));
        }
        
        public class Factory : PlaceholderFactory<ProgressReset>
        {
        }
    }
}