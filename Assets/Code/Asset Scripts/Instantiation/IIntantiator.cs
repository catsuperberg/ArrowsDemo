using System.Collections.Generic;
using UnityEngine;

namespace AssetScripts.Instantiation
{
    public interface IInstatiator
    {
        public IList<GameObject> Instantiated {get;} 
        public GameObject Instantiate(GameObject original, string name = null, Transform parent = null, 
           Vector3? position = null, Quaternion? rotation = null);
        public void UndoImplementationSpecifics();
        public void RedoImplementationSpecifics();
    }
}