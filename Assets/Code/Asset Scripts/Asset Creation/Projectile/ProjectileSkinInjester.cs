using Game.Gameplay.Meta.Skins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

namespace AssetScripts.AssetCreation
{
    public class ProjectileSkinInjester : EditorSkinInjester
    {   
        [SerializeField]
        string _AssetPath;
        [SerializeField]
        string _OutputResourcesPath;
        
        const string _iconizerPrefabPath = "Assets/Code/Asset Scripts/Asset Creation/Iconizer.prefab";
        string _databaseJsonPath => _OutputResourcesPath + "/Projectiles.json";
                  
        override public void InjestSkins()
        {
            var injester = ComposeInjester();
            injester.InjestSkins();
        }
        
        ISkinInjester ComposeInjester()
        {
            var loader = new RawModelLoader();
            var prefabGenerator = new BundleProjectilePrefabGenerator();
            var iconizerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_iconizerPrefabPath);         
            var iconGenerator = new PrefabIconGenerator();        
            iconGenerator.Initialize(iconizerPrefab);                
            var database = new PermanentSkinsDatabase<ProjectileSkinData>(_databaseJsonPath);
            var assetMaker = new ProjectileAssetMaker(loader, prefabGenerator, iconGenerator, (_AssetPath, _OutputResourcesPath));
            var dataEnricher = new InjestDataEnricher<ProjectileSkinData, ProjectileInjestData>(_AssetPath);
            return new SkinInjester<ProjectileSkinData, ProjectileInjestData>(assetMaker, database, dataEnricher);
        }
    }
}

#endif