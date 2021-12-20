using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectFillable
{
    public List<GameObject> TargetObjects {get;}
    
    public void AddObjectToList(GameObject objectToAdd);
}