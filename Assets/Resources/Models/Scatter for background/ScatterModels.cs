using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScatterModels", menuName = "ScriptableObjects/ScatterModels", order = 1)]
public class ScatterModels : ScriptableObject
{
    public List<GameObject> PrefabsVillageStuff;
    public List<GameObject> PrefabsTerrain;
    public List<GameObject> PrefabsVegetation;
    
    public List<List<GameObject>> AllGroups 
        {get => new List<List<GameObject>>{PrefabsVillageStuff, PrefabsTerrain, PrefabsVegetation};} 
}
