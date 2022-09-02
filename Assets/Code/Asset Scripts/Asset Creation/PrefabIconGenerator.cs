
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public class PrefabIconGenerator : MonoBehaviour
    {
        [SerializeField]
        GameObject IconizerPrefab;
        
        Vector3 PositionAwayFromObjects => Vector3.up * 500;
        
        public void Initialize(GameObject iconizerPrefab)
        {
            IconizerPrefab = IconizerPrefab ?? iconizerPrefab;
        }
        
        public Texture2D CreatePrefabPreview(GameObject prefab)
        {
            var prefabGO = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            var iconizerGO = Instantiate(IconizerPrefab, PositionAwayFromObjects, Quaternion.identity);
            var iconizer = iconizerGO.GetComponent<Iconizer>();
            var texture = iconizer.GetIcon(prefabGO);
            DestroyImmediate(iconizerGO);
            DestroyImmediate(prefabGO);
            return texture;
        }
    }
}