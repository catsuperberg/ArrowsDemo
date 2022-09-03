using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public interface ISkinPrefabGenerator 
    {     
        const string _containerTag = "RuntimeResourceContainer";
        static GameObject _container;
        
        public UnityEngine.Object CreateRuntimeResource(GameObject skinObject);
        
        #if UNITY_EDITOR
        public string CreatePrefab(GameObject skinObject, string folderToSaveTo);  
        #endif
             
        static void HideUnderResourceContainer(GameObject instance)
        {
            if(_container == null)
                _container = GameObject.FindWithTag(_containerTag) ?? CreateContainer();
            (instance).transform.SetParent(_container.transform);  
        }
        
        static GameObject CreateContainer()
        {
            var container = new GameObject();
            container.name = _containerTag;
            container.tag = _containerTag;
            container.SetActive(false);
            return container;            
        }
    }
}