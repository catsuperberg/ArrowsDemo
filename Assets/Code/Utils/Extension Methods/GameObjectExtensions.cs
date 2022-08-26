using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{        
    public static class GameObjectExtensions
    {   
        public static GameObject FindChildObjectWithTag(this GameObject parent, string tag)
        {
           Transform t = parent.transform;
           foreach(Transform tr in t)
           {
                  if(tr.tag == tag)
                  {
                       return tr.gameObject;
                  }
           }
           return null;
        }
        
        public static T FindChildComponentWithTag<T>(this GameObject parent, string tag)where T:Component
        {
           Transform t = parent.transform;
           foreach(Transform tr in t)
           {
                  if(tr.tag == tag)
                  {
                       return tr.GetComponent<T>();
                  }
           }
           return null;
        }
    }    
}