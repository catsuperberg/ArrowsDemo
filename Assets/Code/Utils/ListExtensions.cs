using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class ListExtensions
    {            
        public static List<T> InList<T>(this T item)
            => new List<T> { item };
    }
}