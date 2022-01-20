using UnityEngine;

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
        Debug.Log("SCALING RESOLUTION!!!!");
        Scale = scale;
        ScalableBufferManager.ResizeBuffers(Scale, Scale);
    }
}
