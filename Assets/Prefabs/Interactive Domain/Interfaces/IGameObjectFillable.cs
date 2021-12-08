using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectFillable
{
    public List<GameObject> Objects {get;}
    
    public void AddObjectToList(GameObject objectToAdd);
}