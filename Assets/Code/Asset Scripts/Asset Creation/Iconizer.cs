using DataAccess.DiskAccess.GameFolders;
using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace AssetScripts.AssetCreation
{
    public class Iconizer : MonoBehaviour
    {
        [SerializeField]
        Camera IconizerCamera;
        
        [SerializeField]
        GameObject SubjectContainer;
        
        [SerializeField]
        RenderTexture RenderTexture;
        
        float _magicFitCoefficient = 0.6f;  // dependent on camera angle which is determined by camera position
        
        public Texture2D GetIcon(GameObject objectToIconize)
        {
            RenderTexture.active = RenderTexture;
            PositionModelAndRender(objectToIconize);            
            var texture = CopyRenderToNewTexture();            
            RenderTexture.active = null;
            return texture;
        }
        
        void PositionModelAndRender(GameObject objectToIconize)
        {
            objectToIconize.transform.SetParent(SubjectContainer.transform, false);
            objectToIconize.transform.localPosition = Vector3.zero;
            CenterCamera(objectToIconize);
            FitObjectInCameraView(objectToIconize);
            IconizerCamera.Render();
        }
        
        void CenterCamera(GameObject objectToCenterOn)
        {
            var boundsCenter = objectToCenterOn.GetComponent<MeshRenderer>()?.bounds.center ??
                throw new Exception("No MeshRenderer found on object to iconize");
                
            IconizerCamera.transform.LookAt(boundsCenter);
        }
        
        void FitObjectInCameraView(GameObject objectToFit)
        {
            var boundsRadius = objectToFit.GetComponent<MeshRenderer>()?.bounds.extents.magnitude ??
                throw new Exception("No MeshRenderer found on object to iconize");
            var distanceToObject = Vector3.Distance(IconizerCamera.transform.position, objectToFit.transform.position);
            var halfFOV = MathF.Asin(boundsRadius/distanceToObject);
            var halfFOVdegrees = (float)((180 / Math.PI) * halfFOV);
            IconizerCamera.fieldOfView = halfFOVdegrees * 2 * _magicFitCoefficient;
        }
        
        Texture2D CopyRenderToNewTexture()
        {
            var texture = new Texture2D(RenderTexture.width, RenderTexture.height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, RenderTexture.width, RenderTexture.height), 0, 0);
            texture.Apply();
            return texture;
        }
    }
}