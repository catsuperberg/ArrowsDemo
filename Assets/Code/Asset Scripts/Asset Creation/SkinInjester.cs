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
    public class SkinInjester<TSkinData, TInjestData> : ISkinInjester
        where TSkinData : ISkinData<TSkinData>, ISkinDataEnricher<TSkinData, TInjestData> 
        where TInjestData : BasicInjestData
    {
        IAssetMaker<TSkinData> _assetMaker;
        ISkinDatabase<TSkinData> _database;
        InjestDataEnricher<TSkinData, TInjestData> _dataEnricher;
        
        IEnumerable<(string name, string pathToData)> _injestDataToSet;
        IEnumerable<TSkinData> _skinsWithoutInjestData;
        
        public SkinInjester(
            IAssetMaker<TSkinData> assetMaker, ISkinDatabase<TSkinData> database,
            InjestDataEnricher<TSkinData, TInjestData> dataEnricher)
        {
            _assetMaker = assetMaker ?? throw new ArgumentNullException(nameof(assetMaker));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _dataEnricher = dataEnricher ?? throw new ArgumentNullException(nameof(dataEnricher));
        }
        
        public void InjestSkins()
        {
            _skinsWithoutInjestData = _assetMaker.MakeAssetsFromInjestable();
            SetDatabaseEntries();
            AssetDatabase.Refresh();
        }
        
        void SetDatabaseEntries()
        {            
            var dataForSkins = _dataEnricher.GetInjestDataForAllSkins(_skinsWithoutInjestData);
            _database.SetSkinsDataKeepOldPropertiesOnNull(dataForSkins);
            _database.SaveToPermanent();
        }
    }
}

#endif