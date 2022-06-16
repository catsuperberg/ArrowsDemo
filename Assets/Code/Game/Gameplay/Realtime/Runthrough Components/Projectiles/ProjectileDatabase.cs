using AssetScripts.AssetCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{
    [Serializable]
    public class ProjectileDatabase
    {        
        public IList<ProjectileSkinData> Skins {get => ValidSkins().AsReadOnly();}
        [SerializeField]
        List<ProjectileSkinData> _skins;
        
        public void AddSkinsUniqueByName(List<ProjectileSkinData> skinsData)
        {
            if(_skins == null)
                _skins = new List<ProjectileSkinData>();
            var namesAlreadyInList = _skins.Select(instance => instance.Name);
            var uniqueSkins = skinsData.Where(instance => !namesAlreadyInList.Contains(instance.Name));  
            _skins.AddRange(uniqueSkins);
        }
        
        List<ProjectileSkinData> ValidSkins()
        {
            return _skins; // TEMP should check if everything needed for skin is present
        }
    }
}