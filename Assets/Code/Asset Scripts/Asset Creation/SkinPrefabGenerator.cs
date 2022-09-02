using System;
using UnityEngine;
using UnityEditor;

namespace AssetScripts.AssetCreation
{
    public class SkinPrefabGenerator : ISkinPrefabGenerator
    {                     
        public UnityEngine.Object CreateRuntimeResource(GameObject skinObject)
        {
            var prefabResource = Resources.InstanceIDToObject(PrepareGameObject(skinObject).GetInstanceID());
            ISkinPrefabGenerator.HideUnderResourceContainer((GameObject)prefabResource);             
            return prefabResource;
        }
        
        public string CreatePrefab(GameObject skinObject, string folderToSaveTo)
        {          
            PrepareGameObject(skinObject);
            
            SaveModel(skinObject, folderToSaveTo);
            var pathToPrefab = SavePrefab(skinObject, folderToSaveTo);
            return pathToPrefab;
        }
        
        GameObject PrepareGameObject(GameObject skinObject)
        {
            BlenderToGameTransform(skinObject);
            skinObject.SetActive(true);
            return skinObject;
        }

        void SaveModel(GameObject modelObject, string folder)
        {
            var textureName = "texture_" + modelObject.name + ".asset";
            var materialName = "material_" + modelObject.name + ".asset";
            var meshName = "mesh_" + modelObject.name + ".asset";
            var renderer = modelObject.GetComponent<Renderer>();
            var material = renderer.sharedMaterial;
            var mesh = modelObject.GetComponent<MeshFilter>().sharedMesh;
            var texture = material.GetTexture("_MainTex");
                   
            // Order is relevant, material wouldn't have texture if texture is't saved before it     
            if(texture != null)  
                AssetDatabase.CreateAsset(texture, System.IO.Path.Combine(folder, textureName));  
            AssetDatabase.CreateAsset(material, System.IO.Path.Combine(folder, materialName));    
            AssetDatabase.CreateAsset(mesh, System.IO.Path.Combine(folder, meshName));        
        }      
                
        string SavePrefab(GameObject prefab, string folder)
        {
            bool prefabSuccess;            
            var fullPath = System.IO.Path.Combine(folder, prefab.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(prefab, fullPath, out prefabSuccess);
            if(!prefabSuccess)
                throw new Exception("Couldn't save created asset as prefab");
            
            return fullPath.Replace("Assets/Prefabs/Gameplay Items/Projectiles/Resources/", "");
        }
        
        void BlenderToGameTransform(GameObject GOtoFix)
        {
            GOtoFix.transform.position = Vector3.zero;
            GOtoFix.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}