using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Realtime.GeneralUseInterfaces
{    
    public interface IGameObjectFillable
    {
        public List<GameObject> TargetObjects {get;}
        
        public void AddObjectToList(GameObject objectToAdd);
    }
}