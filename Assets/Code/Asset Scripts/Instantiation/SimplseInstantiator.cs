using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetScripts.Instantiation
{
    public class SimpleInstantiator : IInstatiator
    {
        public IList<GameObject> Instantiated {get {return _instantiated.AsReadOnly();}}
        List<GameObject> _instantiated = new List<GameObject>(); 
        
        public GameObject Instantiate(GameObject original, string name = null, Transform parent = null, 
            Vector3? position = null, Quaternion? rotation = null)
        {
            var initialPosition = (position != null) ? position.Value : Vector3.zero;
            var initialRotation = (rotation != null) ? rotation.Value : Quaternion.identity;
            var instance = GameObject.Instantiate(original, initialPosition, initialRotation, parent);
            if(name != null)
                instance.name = name;
            _instantiated.Add(instance);
            
            return instance;
        }
            
        public async Task UndoImplementationSpecifics()
        {
            throw new System.Exception("This implementation of IInstantiator has nothing to undo");
        }
        
        public void RedoImplementationSpecifics()
        {
            throw new System.Exception("This implementation of IInstantiator has nothing to redo");
        }
    }
}