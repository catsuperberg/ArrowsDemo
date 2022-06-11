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
        
        public void CreateBundleProjectilesPrefab(GameObject skinObject)
        {          
            var projectile = Instantiate(TemplatePrefab);
            projectile.name = "Bundle_" + skinObject.name;
            var projectileAsset = projectile.transform.Find("Projectile Asset");
            BlenderToGameTransform(skinObject);
            skinObject.transform.SetParent(projectileAsset, false);
            skinObject.SetActive(true);
            skinObject.AddComponent<FlyingMovement>();
            var bundleScript = projectile.GetComponent<IProjectile>();
            
            var resourcesFolder = "Assets/Prefabs/Gameplay Items/Projectiles/Resources";
            var skinFolderName = projectile.name;      
            var skinFolder = System.IO.Path.Combine(resourcesFolder, skinFolderName);
            AssetDatabase.CreateFolder(resourcesFolder, skinFolderName);
            
            SaveModel(skinObject, skinFolder);
            SavePrefab(projectile, skinFolder);
        }
        
        void SavePrefab(GameObject prefab, string folder)
        {
            Debug.LogWarning("Saving asset as prefab: " + prefab);
            bool prefabSuccess;            
            PrefabUtility.SaveAsPrefabAsset(prefab, System.IO.Path.Combine(folder, prefab.name + ".prefab"), out prefabSuccess);
            if(!prefabSuccess)
                throw new Exception("Couldn't save created asset as prefab");
        }

        void SaveModel(GameObject modelObject, string folder)
        {
            Debug.LogWarning("Saving model: " + modelObject);
            var textureName = "texture_" + modelObject.name + ".asset";
            var materialName = "material_" + modelObject.name + ".asset";
            var meshName = "mesh_" + modelObject.name + ".asset";
            var renderer = modelObject.GetComponent<Renderer>();
            Material material = renderer.material;
            var shaderMaterial = renderer.sharedMaterial;
            var mesh = modelObject.GetComponent<MeshFilter>().mesh;
            var texture = shaderMaterial.GetTexture("_MainTex");
                        
            if(texture != null)
                AssetDatabase.CreateAsset(texture, System.IO.Path.Combine(folder, textureName)); 
            AssetDatabase.CreateAsset(material, System.IO.Path.Combine(folder, materialName)); 
            AssetDatabase.CreateAsset(mesh, System.IO.Path.Combine(folder, meshName));              
        }      
        
        void BlenderToGameTransform(GameObject GOtoCorrect)
        {
            GOtoCorrect.transform.position = Vector3.zero;
            GOtoCorrect.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}