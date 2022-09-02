using AssetScripts.Movement;
using System;
using UnityEngine;
using UnityEditor;
using Utils;

namespace AssetScripts.AssetCreation
{
    public class BundleProjectilePrefabGenerator : ISkinPrefabGenerator
    {             
        const string _bundlePrefabPath = "Assets/Prefabs/Gameplay Items/Projectiles/Template/Resources/BundleTemplate.prefab";
        
        public UnityEngine.Object CreateRuntimeResource(GameObject skinObject)
        {
            var instantiatedGOs = PrepareGameObject(skinObject);
            var resource = Resources.InstanceIDToObject(instantiatedGOs.fullPrefab.GetInstanceID());
            ISkinPrefabGenerator.HideUnderResourceContainer((GameObject)resource);
            ISkinPrefabGenerator.HideUnderResourceContainer((GameObject)skinObject);
            return resource;
        }
        
        public string CreatePrefab(GameObject skinObject, string skinFolder)
        {          
            var instantiatedGOs = PrepareGameObject(skinObject);
            
            SaveModel(instantiatedGOs.tempSkin, skinFolder);
            SavePrefab(instantiatedGOs.tempSkin, skinFolder);
            var pathToProjectile = SavePrefab(instantiatedGOs.fullPrefab, skinFolder);
            GameObject.DestroyImmediate(instantiatedGOs.fullPrefab);
            return pathToProjectile;
        }        
                
        (GameObject fullPrefab, GameObject tempSkin) PrepareGameObject(GameObject skinObject)
        {
            // #if UNITY_EDITOR
            //     var templatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_bundlePrefabPath);
            //     var projectile = GameObject.Instantiate(templatePrefab);
            // #endif
            // #if !UNITY_EDITOR
            //     var projectile = GameObject.Instantiate(Resources.Load<GameObject>(_bundlePrefabPath.GetAtResourcesWithNoExtension())); 
            // #endif
            // var tempResource = Resources.Load<UnityEngine.Object>(_bundlePrefabPath.GetAtResourcesWithNoExtension());
            // var projectile = GameObject.Instantiate(tempResource);
            var projectile = GameObject.Instantiate(Resources.Load<GameObject>(_bundlePrefabPath.GetAtResourcesWithNoExtension())); 
            var tempSkinGO = GameObject.Instantiate(skinObject);
            tempSkinGO.name = skinObject.name;
            projectile.name = "Bundle_" + tempSkinGO.name;
            var projectileAsset = projectile.transform.Find("Projectile Asset");
            BlenderToGameTransform(tempSkinGO);
            tempSkinGO.transform.SetParent(projectileAsset, false);
            tempSkinGO.SetActive(true);
            tempSkinGO.AddComponent<FlyingMovement>();
            return (projectile, tempSkinGO);
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
        void BlenderToGameTransform(GameObject GOtoCorrect)
        {
            GOtoCorrect.transform.position = Vector3.zero;
            GOtoCorrect.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}