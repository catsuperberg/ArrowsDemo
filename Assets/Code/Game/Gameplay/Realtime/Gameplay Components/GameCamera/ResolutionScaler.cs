using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents.GameCamera
{
    [ExecuteInEditMode]
    public class ResolutionScaler : MonoBehaviour
    { 
        [SerializeField]
        private float Scale = 1f;
        
        void OnValidate()
        {
            SetScale(Scale);
        }
        
        public void SetScale(float scale)
        {       
            Scale = Mathf.Clamp(scale, 0.25f, 1f);
            ScalableBufferManager.ResizeBuffers(Scale, Scale);
        }
    }
}
