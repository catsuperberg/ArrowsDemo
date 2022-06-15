using AssetScripts.Movement;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using UnityEngine;
using UnityEditor;

namespace AssetScripts.AssetCreation
{
    public class BundleProjectilePrefabGenerator : MonoBehaviour
    {     
        [SerializeField]
        GameObject TemplatePrefab;
        
        public string CreateBundleProjectilesPrefab(GameObject skinObject, string skinFolder)
        {          
            var projectile = Instantiate(TemplatePrefab);
            projectile.name = "Bundle_" + skinObject.name;
            var projectileAsset = projectile.transform.Find("Projectile Asset");
            BlenderToGameTransform(skinObject);
            skinObject.transform.SetParent(projectileAsset, false);
            skinObject.SetActive(true);
            skinObject.AddComponent<FlyingMovement>();
            var bundleScript = projectile.GetComponent<IProjectile>();
            
            SaveModel(skinObject, skinFolder);
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