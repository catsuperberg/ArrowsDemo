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
    public class CrossbowSkinInjester : EditorSkinInjester
    {   
        [SerializeField]
        string _AssetPath;
        [SerializeField]
        string _OutputResourcesPath;
        
        const string _iconizerPrefabPath = "Assets/Code/Asset Scripts/Asset Creation/Iconizer.prefab";
        string _databaseJsonPath => _OutputResourcesPath + "/Crossbows.json";
        
        override public void InjestSkins()
        {
            var injester = ComposeInjester();
            injester.InjestSkins();
        }
        
        ISkinInjester ComposeInjester()
        {
            var loader = new RawModelLoader();
            var prefabGenerator = new SkinPrefabGenerator();
            var iconizerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_iconizerPrefabPath);         
            var iconGenerator = new PrefabIconGenerator();        
            iconGenerator.Initialize(iconizerPrefab);                
            var database = new PermanentSkinsDatabase<CrossbowSkinData>(_databaseJsonPath);
            var assetMaker = new CrossbowAssetMaker(loader, prefabGenerator, iconGenerator, (_AssetPath, _OutputResourcesPath));
            var dataEnricher = new InjestDataEnricher<CrossbowSkinData, CrossbowInjestData>(_AssetPath);
            return new SkinInjester<CrossbowSkinData, CrossbowInjestData>(assetMaker, database, dataEnricher);
        }
    }
}

#endif