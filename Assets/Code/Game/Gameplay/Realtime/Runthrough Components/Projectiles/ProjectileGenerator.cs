using AssetScripts.Instantiation;
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
        
        public GameObject CreateArrows(BigInteger initialCount, float movementWidth, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new System.ArgumentNullException("IInstatiator isn't provided for: " + this.GetType().Name);
                
            var bundle = assetInstatiator.Instantiate(arrowBundle, name: "Projectile (Arrow bundle)");
            var bundleScript = bundle.GetComponent<IProjectile>();
            if(bundleScript != null)
            {
                bundleScript.Initialize(initialCount, movementWidth, collisionEnabled: false);
                return bundle;                
            }
            else
            {
                throw new System.Exception("No IProjectileObject in selected prefab");
                // Debug.LogWarning("No IProjectileObject in selected prefab");
                // return null;
            }
                
        }
    }
}
