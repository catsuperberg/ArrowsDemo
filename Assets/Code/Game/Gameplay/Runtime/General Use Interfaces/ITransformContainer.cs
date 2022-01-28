using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Runtime.GeneralUseInterfaces
{    
    public interface ITransformContainer
    {
        public Transform MainTransform {get;}
        public List<Transform> ChildrenTransforms {get;}
    }
}