using AssetScripts.Movement;
using DataAccess.DiskAccess.GameFolders;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEditor;

namespace AssetScripts.AssetCreation
{
    public class ProjectilePrefabCreator : MonoBehaviour
    {     
        [SerializeField]
        GameObject TemplatePrefab;
        
        ProjectileRawModelLoader _modelLoader;
          
//         [Inject]
//         public void construct(ProjectileRawModelLoader modelLoader)
//         {            
//             if(modelLoader == null)
//                 throw new ArgumentNullException("ProjectileRawModelLoader isn't provided to " + this.GetType().Name);
            
//             _modelLoader = modelLoader;
//             CreateBundleProjectilesPrefabs();           
//         }
        
//         void CreateBundleProjectilesPrefabs()
//         {          
//             var pencilGO = _modelLoader.LoadPencil();
//             var projectile = Instantiate(TemplatePrefab);
//             projectile.name = "PencilBundle";
//             projectile.transform.position = Vector3.left * 80;
//             var projectileAsset = projectile.transform.Find("Projectile Asset");
//             BlenderToGameTransform(pencilGO);
//             pencilGO.transform.SetParent(projectileAsset, false);
//             pencilGO.SetActive(true);
//             pencilGO.AddComponent<FlyingMovement>();
//             var bundleScript = projectile.GetComponent<IProjectile>();
//             if(Application.isEditor)
//                 SaveModel(pencilGO);
//             if(Application.isEditor)
//                 SavePrefab(projectile);
//             bundleScript.Initialize(20, 12, false); 
//         }
//  #if UNITY_EDITOR    
//         void SavePrefab(GameObject prefab)
//         {
//             Debug.LogWarning("Saving asset as prefab: " + prefab);
//             bool prefabSuccess;
//             PrefabUtility.SaveAsPrefabAsset(prefab, "Assets/Prefabs/Gameplay Items/Projectiles/" + prefab.name + ".prefab", out prefabSuccess);
//             if(!prefabSuccess)
//                 throw new Exception("Couldn't save created asset as prefab");
//         }
        
//         void SaveModel(GameObject prefab)
//         {
//             Debug.LogWarning("Saving model: " + prefab);
//             var textureName = "texture_" + prefab.name + ".asset";
//             var materialName = "material_" + prefab.name + ".asset";
//             var meshName = "mesh_" + prefab.name + ".asset";
//             var renderer = prefab.GetComponent<Renderer>();
//             Material material = renderer.material;
//             var shaderMaterial = renderer.sharedMaterial;
//             var texture = shaderMaterial.GetTexture("_MainTex");
//             var mesh = prefab.GetComponent<MeshFilter>().mesh;
            
//             AssetDatabase.CreateAsset(texture, "Assets/Prefabs/Gameplay Items/Projectiles/" + textureName); 
//             AssetDatabase.CreateAsset(material, "Assets/Prefabs/Gameplay Items/Projectiles/" + materialName); 
//             AssetDatabase.CreateAsset(mesh, "Assets/Prefabs/Gameplay Items/Projectiles/" + meshName);              
//         }      
        
//  #endif
        
//         void BlenderToGameTransform(GameObject GOtoCorrect)
//         {
//             GOtoCorrect.transform.position = Vector3.zero;
//             GOtoCorrect.transform.rotation = Quaternion.Euler(0, 90, 0);
//         }
        
    }
}