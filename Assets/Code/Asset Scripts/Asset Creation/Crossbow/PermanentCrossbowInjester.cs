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
    public class PermanentCrossbowInjester : MonoBehaviour
    {           
        [SerializeField]
        string injestFolder;
        
        SkinInjester<CrossbowSkinData, CrossbowInjestData> _injester;
        
        void CreateInjester()
        {            
            // var _loader = modelLoader;
            // var _prefabGenerator = prefabGenerator;   
            // var _iconGenerator = iconGenerator;        
            // var assetMaker = new CrossbowAssetMaker(_loader, prefabGenerator, iconGenerator, (folderWithSkins, finalResourceFolder));
            
            // var _dataEnricher = new InjestDataEnricher<CrossbowSkinData, CrossbowInjestData>(finalResourceFolder);
            
            // _injester = new SkinInjester<CrossbowSkinData, CrossbowInjestData>(assetMaker, database, _dataEnricher);
        }
        
        public void InjestCrossbows()
        {
            CreateInjester();
            _injester.InjestSkins();
        }
    }
}

#endif