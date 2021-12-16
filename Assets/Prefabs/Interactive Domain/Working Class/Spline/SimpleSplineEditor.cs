// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(SimpleSpline))]
// public class SimpleSplineEditor : Editor 
// {
//      private SimpleSpline spline { get { return (SimpleSpline)serializedObject.targetObject; } }
     
//      void OnSceneGUI() 
//      {
//             Handles.DrawBezier(spline._startPosition,
//                 spline._endPosition,
//                 spline._enterHandlePosition,
//                 spline._exitHandlePosition,
//                 new Color(0.8f, 0.8f, 0.8f),
//                 null,
//                 4);
//      }
// }