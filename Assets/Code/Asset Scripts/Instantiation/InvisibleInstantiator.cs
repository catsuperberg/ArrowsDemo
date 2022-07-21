using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            
        public async Task UndoImplementationSpecifics()
        {
            LeaveOnlyValidInstances();
            var semaphore = new SemaphoreSlim(0, 1); 
            UnityMainThreadDispatcher.Instance().Enqueue(() => {Undo(); semaphore.Release();});   
            await semaphore.WaitAsync();    
        }
        
        void Undo()
        {       
            foreach(var renderer in _hidenRenderers)
                renderer.enabled = true;        
        }
        
        public void RedoImplementationSpecifics()
        {
            LeaveOnlyValidInstances();
            UnityMainThreadDispatcher.Instance().Enqueue(() => {Redo();});        
        }
        
        void Redo()
        {       
            foreach(var renderer in _hidenRenderers)
                renderer.enabled = false;  
        }
        
        void LeaveOnlyValidInstances()
        {
            _instantiated = _instantiated.Where(instance => instance != null).ToList();
            _hidenRenderers = _hidenRenderers.Where(instance => instance != null).ToList();
        }
    }
}