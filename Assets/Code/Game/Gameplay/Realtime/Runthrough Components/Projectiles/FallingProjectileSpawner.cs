using AssetScripts.Movement;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{
    public class FallingProjectileSpawner
    {
        GameObject _spawnerBaseGO;
        IDamageableWithTransforms _projectile;
        IDamageableWithTransforms _target;        
        
        int _maxAnimatedProjectiles = 300;
        List<GameObject> _animatedProjectiles = new List<GameObject>();
        
        public FallingProjectileSpawner(IDamageableWithTransforms projectile, IDamageableWithTransforms target)
        {     
             if(projectile == null)
                throw new System.Exception("IDamageableWithTransforms 'projectile' isn't provided to FinishingScene");
             if(target == null)
                throw new System.Exception("IDamageableWithTransforms 'target' isn't provided to FinishingScene");    
                        
            _spawnerBaseGO = new GameObject("Falling Projectile Spawner");
            _projectile = projectile;
            _target = target;            
        }       
        
        public void SpawnFlyingProjectiles()
        {
            var projectileCount = (_animatedProjectiles.Any()) ? _animatedProjectiles.Count : 0;
            if(ThereAreProjectilesToSpawn() && projectileCount <= _maxAnimatedProjectiles && DecisionToSpawn())
            {
                Transform targetTransform = null;
                Transform projectileTransform = null;
                
                targetTransform = _randomTargetTransform;
                projectileTransform = _randomProjectileTransform;
               
                var moverGameObject = new GameObject("Ballistic mover");
                moverGameObject.transform.SetParent(_spawnerBaseGO.transform);
                var startTransform = projectileTransform;          
                var mover = moverGameObject.AddComponent<BallisticMover>();
                var projectile = MonoBehaviour.Instantiate((_projectile as IProjectile).ProjectilePrefab, Vector3.zero, Quaternion.identity);
                projectile.SetActive(true);
                mover.initialize(startTransform, targetTransform, 0.85f);
                mover.StartMover(200f);
                projectile.transform.SetParent(moverGameObject.transform, false);
                _animatedProjectiles.Add(moverGameObject);                    
            }
        }   
        
        public void Destroy()
        {
            GameObject.Destroy(_spawnerBaseGO);
        }
        
        bool ThereAreProjectilesToSpawn()
        {
            return _projectile.DamagePoints >= 1 && _projectile != null;
        }
        
        bool DecisionToSpawn()
        {
            return Random.Range(1, 20) > 3;
        }
        
        Transform _randomProjectileTransform
        {
            get
            {
                if(_projectile.ChildrenTransforms.Any())
                {                    
                    var projectileIndex = Random.Range(0, _projectile.ChildrenTransforms.Count);     
                    return _projectile.ChildrenTransforms[projectileIndex];
                }
                else
                    return _projectile.MainTransform;
            }
        }
        
        Transform _randomTargetTransform
        {
            get
            {
                var targetCount = _target.ChildrenTransforms.Count;
                Transform tempTransform;
                if(targetCount <=0)
                    tempTransform = _target.MainTransform;
                else                    
                    tempTransform = _target.ChildrenTransforms[Random.Range(0, targetCount)];
                return ModifyToLookDown(tempTransform);
            }
        }
        
        Transform ModifyToLookDown(Transform sourceTransform)
        {
            var tempObj = new GameObject("temp object, for transform only (ModifyToLookDown)");                
            tempObj.transform.position = sourceTransform.position;
            tempObj.transform.rotation = Quaternion.LookRotation(Vector3.down);
            var tempTransform = tempObj.transform;
            GameObject.Destroy(tempObj);
            return tempTransform;
        }
    }
}

