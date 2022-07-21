using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetScripts.Instantiation
{
    public interface IInstatiator
    {
        public IList<GameObject> Instantiated {get;} 
        public GameObject Instantiate(GameObject original, string name = null, Transform parent = null, 
           Vector3? position = null, Quaternion? rotation = null);
        public Task UndoImplementationSpecifics();
        public void RedoImplementationSpecifics();
    }
}