using AssetScripts.Movement;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace AssetScripts.AssetCreation
{
    public class BundleProjectilePrefabGenerator : MonoBehaviour, ISkinPrefabGenerator
    {             
        const string _bundlePrefabPath = "Assets/Prefabs/Gameplay Items/Projectiles/BundleTemplate.prefab";
        
        public string CreatePrefab(GameObject skinObject, string skinFolder)
        {          
            var templatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_bundlePrefabPath);
            var projectile = Instantiate(templatePrefab);
            var tempSkinGO = Instantiate(skinObject);
            tempSkinGO.name = skinObject.name;
            projectile.name = "Bundle_" + tempSkinGO.name;
            var projectileAsset = projectile.transform.Find("Projectile Asset");
            BlenderToGameTransform(tempSkinGO);
            tempSkinGO.transform.SetParent(projectileAsset, false);
            tempSkinGO.SetActive(true);
            tempSkinGO.AddComponent<FlyingMovement>();
            
            SaveModel(tempSkinGO, skinFolder);
            SavePrefab(tempSkinGO, skinFolder);
            var pathToProjectile = SavePrefab(projectile, skinFolder);
            DestroyImmediate(projectile);
            return pathToProjectile;
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
#endif