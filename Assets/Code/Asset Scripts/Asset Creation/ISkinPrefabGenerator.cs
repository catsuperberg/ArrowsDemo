using AssetScripts.Movement;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace AssetScripts.AssetCreation
{
    public interface ISkinPrefabGenerator 
    {     
        public string CreatePrefab(GameObject skinObject, string folderToSaveTo);  
    }
}
#endif