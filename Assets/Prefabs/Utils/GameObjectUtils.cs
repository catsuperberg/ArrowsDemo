using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class GameObjectUtils
    {            
        // capsule only
        public static float GetAreaOfObjectsWithColliders(List<GameObject> objectsWithColiders)
        {
            var area = 0.0f;
            foreach(GameObject entry in objectsWithColiders)
            {
                var capsule = entry.GetComponent<CapsuleCollider>();
                var bounds = capsule.bounds;
                var instanceArea =bounds.size.x*bounds.size.z;
                area += instanceArea;
            }            
            return area;
        } 
    }    
}