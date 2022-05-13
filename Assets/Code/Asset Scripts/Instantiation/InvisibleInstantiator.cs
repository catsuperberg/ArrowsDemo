using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssetScripts.Instantiation
{
    public class InvisibleInstantiator : IInstatiator
    {
        public IList<GameObject> Instantiated {get {return _instantiated.AsReadOnly();}}
        List<GameObject> _instantiated = new List<GameObject>(); 
        List<MeshRenderer> _hidenRenderers = new List<MeshRenderer>();
        
        public GameObject Instantiate(GameObject original, string name = null, Transform parent = null, 
            Vector3? position = null, Quaternion? rotation = null)
        {
            var initialPosition = (position != null) ? position.Value : Vector3.zero;
            var initialRotation = (rotation != null) ? rotation.Value : Quaternion.identity;
            var instance = GameObject.Instantiate(original, initialPosition, initialRotation, parent);
            if(name != null)
                instance.name = name;
            _instantiated.Add(instance);
            
            HideVisualisableComponents(instance);
            return instance;
        }
            
        void HideVisualisableComponents(GameObject instance)
        {
            var meshRenderers = instance.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in meshRenderers.Where(component => component.enabled))
            {
                renderer.enabled = false;            
                _hidenRenderers.Add(renderer);  
            }
        }
            
        public void UndoImplementationSpecifics()
        {
            foreach(var renderer in _hidenRenderers)
                renderer.enabled = true;
        }
        
        public void RedoImplementationSpecifics()
        {
            foreach(var renderer in _hidenRenderers)
                renderer.enabled = false;            
        }
    }
}