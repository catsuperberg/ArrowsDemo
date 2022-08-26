using DataManagement;
using System;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Crossbow
{    
    public class CrossbowInPlaceReplacer
    {                
        ICrossbowProvider _crossbowGenerator;
        IRegistryValueReader _userContextRegistry;

        public CrossbowInPlaceReplacer(ICrossbowProvider crossbowGenerator, [Inject(Id = "userRegistryAccessor")] IRegistryValueReader userContextRegistry)
        {
            _crossbowGenerator = crossbowGenerator ?? throw new ArgumentNullException(nameof(crossbowGenerator));
            _userContextRegistry = userContextRegistry ?? throw new ArgumentNullException(nameof(userContextRegistry));
        }
        
        public GameObject CreateNewSelectedCrossbow() => _crossbowGenerator.CreateSelected();
    }
}
