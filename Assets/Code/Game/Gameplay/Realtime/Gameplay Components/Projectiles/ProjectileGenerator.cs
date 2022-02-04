using System.Numerics;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{    
    public class ProjectileGenerator : MonoBehaviour, IProjectileProvider
    {        
        [SerializeField]
        private GameObject arrowBundle;
        
        public GameObject CreateArrows(BigInteger initialCount, float movementWidth)
        {
            var bundle = Instantiate(arrowBundle, Vector3.zero, Quaternion.identity);
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript != null)
            {
                bundleScript.Initialize(initialCount, movementWidth);
                return bundle;                
            }
            else
            {
                Debug.LogWarning("No IProjectileObject in selected prefab");
                return null;
            }
                
        }
    }
}
