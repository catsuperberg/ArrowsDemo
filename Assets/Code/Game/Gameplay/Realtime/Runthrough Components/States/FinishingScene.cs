using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using GameMath;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents
{
    public enum ResultType
    {   
        Fail,
        Exact,
        Overkill,
        Blank
    }
        
    public class FinishingScene : MonoBehaviour, IFinishNotification
    {        
        FallingProjectileSpawner _projectileSpawner;
        IDamageable _largerDamageable;
        IDamageable _smallerDamageable;
        IDamageableWithTransforms _projectile;
        IDamageableWithTransforms _target;
        RewardCalculator _reward;          
        
        enum State
        {   
            SprayUntilAnyIsZero,
            Overkill,
            Finished,
            Blank
        }
        
        public event EventHandler OnFinished;
        
        double _sprayingArrowsTime = 3;
        double _overkillTime = 1;
        double _overkillSpeed;
        ResultType _result = ResultType.Blank;
        State _state = State.Blank;
        List<State> _statesToGoThrough = new List<State>();
        List<State>.Enumerator _stateEnumerator;
        ExponentialCountCalculator _damageCalculator;
                        
        public void StartScene(IDamageableWithTransforms projectile, IDamageableWithTransforms target, RewardCalculator reward)
        {     
             if(projectile == null)
                throw new ArgumentNullException("IDamageableWithTransforms 'projectile' isn't provided to" + this.GetType().Name);
             if(target == null)
                throw new ArgumentNullException("IDamageableWithTransforms 'target' isn't provided to" + this.GetType().Name);   
             if(reward == null)
                throw new ArgumentNullException("RewardCalculator isn't provided to" + this.GetType().Name);    
                        
            _projectile = projectile;
            _target = target;          
            _reward = reward; 
            _result = CheckResult();  
            _projectileSpawner = new FallingProjectileSpawner(_projectile, _target);
            
            PointCameraAtTarget();
                         
            AssembleStateOrder(_result == ResultType.Overkill);    
            SetValuesToDecay();     
            SetDecaySpeeds(); 
            Task.Run(() => {CreateCalculator(); RunScene();});  // HACK waiting to create HalfLifeCountCalculator, only than changing state    
        }
        
        void PointCameraAtTarget()
        {
            var activeProjectile = _projectile.MainTransform.gameObject;
            var newCameraTarget = new GameObject("CameraTarget");
            newCameraTarget.transform.SetParent(gameObject.transform);
            var projectileTransform = activeProjectile.GetComponentInChildren<TMPro.TMP_Text>().gameObject.transform;
            newCameraTarget.transform.position = projectileTransform.position + new UnityEngine.Vector3(0, 14, 6);
            var additionalRotation =  UnityEngine.Quaternion.Euler(25, 0, 0);
            newCameraTarget.transform.rotation = projectileTransform.rotation * additionalRotation;
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = newCameraTarget.transform;
        }
        
        ResultType CheckResult()
        {
            var diff = _projectile.DamagePoints - _target.DamagePoints;
            if(diff < 0)
                return ResultType.Fail;
            else if(diff == 0)
                return ResultType.Exact;
            else if(diff > 0)
                return ResultType.Overkill;
            return ResultType.Blank;
        }
        
        void AssembleStateOrder(bool projectilesAreOverkillForTargets)
        {
            _statesToGoThrough.Add(State.SprayUntilAnyIsZero);
            if(_result == ResultType.Overkill)
                _statesToGoThrough.Add(State.Overkill);
            _statesToGoThrough.Add(State.Finished);
            _stateEnumerator = _statesToGoThrough.GetEnumerator();
        }
        
        void SetValuesToDecay()
        {
            switch (_result)
            {
                case ResultType.Fail:
                case ResultType.Exact:
                    _smallerDamageable = _projectile as IDamageable;
                    _largerDamageable = _target as IDamageable;
                    break;
                case ResultType.Overkill:
                    _smallerDamageable = _target as IDamageable;
                    _largerDamageable = _projectile as IDamageable;
                    break;
            }             
        }
        
        void SetDecaySpeeds()
        {
            _overkillSpeed = (double)(_projectile.DamagePoints - _target.DamagePoints)/_overkillTime; 
        }
        
        void CreateCalculator()
        {
            _damageCalculator =  new ExponentialCountCalculator(_smallerDamageable.DamagePoints, 0, _sprayingArrowsTime);
        }
        
        void RunScene()
        {            
            AdvanceSceneState();           
        }
        
        void Update()
        {
            switch (_state)
            {
                case State.SprayUntilAnyIsZero:
                    DecreaseCountExponentially(_smallerDamageable);
                    break;                    
                case State.Overkill:
                    DecreaseCountConstantly(_largerDamageable, _overkillSpeed);                                 
                    break;
                case State.Finished:
                    _projectileSpawner.Destroy();
                    OnFinished?.Invoke(this, EventArgs.Empty);
                    break;
            }  
            _projectileSpawner.SpawnFlyingProjectiles();          
        } 
        
        void DecreaseCountExponentially(IDamageable decayTarget)
        {
            var damage = _damageCalculator.GetDeltaForGivenTime(decayTarget.DamagePoints, Time.deltaTime);
            if(_target.DamagePoints > 0)
                _target.Damage(damage);
            _projectile.Damage(damage);
            if(decayTarget.DamagePoints <= 0)
                AdvanceSceneState();            
            _reward.IncreaseReward(damage);     
        }
        
        void DecreaseCountConstantly(IDamageable decayTarget, double speed)
        {
            var damage = new BigInteger(Time.deltaTime*speed);
            damage = (damage >= 1) ? damage : 1;            
            var delta = decayTarget.DamagePoints - damage;
            if(delta <= 0)
            {
                damage += delta;
                AdvanceSceneState();                               
            } 
            if(_target.DamagePoints > 0)
                _target.Damage(damage);
            _projectile.Damage(damage);   
            _reward.IncreaseReward(damage);          
        }
        
        void AdvanceSceneState()
        {
            _stateEnumerator.MoveNext();
            _state = _stateEnumerator.Current; 
        }
    }
}
