using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using GameMath;
using System;
using System.Collections;
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
        bool _onePerIsRunning;
        DateTime _timeStarted;
        ResultType _result = ResultType.Blank;
        State _state = State.Blank;
        List<State> _statesToGoThrough = new List<State>();
        List<State>.Enumerator _stateEnumerator;
        ExponentialCountCalculator _damageCalculator;
        
        delegate BigInteger DecreaseCountDelegate();
        DecreaseCountDelegate _decreaseCount;
                        
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
            _onePerIsRunning = false;
            
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
            _timeStarted = DateTime.Now;    
            AdvanceSceneState();           
        }
        
        void Update()
        {
            switch (_state)
            {
                case State.SprayUntilAnyIsZero:
                case State.Overkill:
                    DecreaseCount();
                    break;
                default:
                    break;
            }          
        } 
        
        void DecreaseCount()
        {       
            if(_decreaseCount == null)
                return;        
            BigInteger damage = _decreaseCount();      
            if(damage > 0)                 
                _projectileSpawner.SpawnFlyingProjectiles();              
        }
        
        void AdvanceSceneState()
        {
            _stateEnumerator.MoveNext();
            _state = _stateEnumerator.Current; 
            _decreaseCount = DecreaseDelegateForTheState();
            if(_state == State.Finished)  
                StartCoroutine(FinishSceneAfterDelay(0.2f));                  
        }        
                
        IEnumerator FinishSceneAfterDelay(float time)
        {
            yield return new WaitForSeconds(time);

            _projectileSpawner.Destroy();
            OnFinished?.Invoke(this, EventArgs.Empty);
        }  
        
        DecreaseCountDelegate DecreaseDelegateForTheState()
        {
            switch (_state)
            {
                case State.SprayUntilAnyIsZero:
                    return DecreaseBeforeOneIsSpentExpontntially;
                case State.Overkill:
                    return DecreaseAfterOneIsSpent;
                case State.Finished:
                    return null;
                default:
                    return null;
            }
        }
        
        BigInteger DecreaseBeforeOneIsSpentExpontntially()
        {
            var damage = GetFrameCountExponentially(_smallerDamageable);  
            if(damage <= 1)
                SwitchToConstantDecreaseDelegate();
                
            AdvanceStateIfDecayTargetBeSpent(_smallerDamageable, damage); 
            DealDamage(_smallerDamageable, damage);
            return damage;
        }        
        
        void SwitchToConstantDecreaseDelegate()
        {
            var timeLeftSpraying = _sprayingArrowsTime - (DateTime.Now - _timeStarted).TotalSeconds; 
            if (_result != ResultType.Overkill && GetConstantDamage(_overkillTime) < 5)
            {
                _decreaseCount = null;
                StartOnePerCoroutineIfNotActive(_smallerDamageable, timeLeftSpraying);    
            }
            else   
            {             
                _decreaseCount = () => {return DecreaseBeforeOneIsSpentConstant(timeLeftSpraying);};
                _decreaseCount();
            }
        }      
                
        BigInteger DecreaseBeforeOneIsSpentConstant(double timeLeft)
        {
            var speed = (double)(_smallerDamageable.DamagePoints)/timeLeft;
            var damage = GetFrameCountConstantSpeed(_smallerDamageable, speed);
            AdvanceStateIfDecayTargetBeSpent(_smallerDamageable, damage); 
            DealDamage(_smallerDamageable, damage); 
            return damage;
        }
        
        BigInteger DecreaseAfterOneIsSpent()
        {
            var damage = GetFrameCountConstantSpeed(_largerDamageable, _overkillSpeed); 
            if(damage <= 1)
                UpdateAfterOneIsZeroDelegate();
                
            AdvanceStateIfDecayTargetBeSpent(_largerDamageable, damage);  
            DealDamage(_largerDamageable, damage);
            return damage;
        }        
        
        void UpdateAfterOneIsZeroDelegate()
        {
            _decreaseCount = null;
            StartOnePerCoroutineIfNotActive(_largerDamageable, _overkillTime); 
        }    
        
        void StartOnePerCoroutineIfNotActive(IDamageable decayTarget, double time)
        {
            var timeLeft = time;  
            var waitPer = Math.Clamp(timeLeft, 0, 1.8) / (double)decayTarget.DamagePoints;    
            StartCoroutine(OnePerCoroutine(decayTarget, waitPer));
        }
        
        IEnumerator OnePerCoroutine(IDamageable decayTarget, double waitSeconds)
        {
            while(decayTarget.DamagePoints > 0) 
            { 
                if(_target.DamagePoints > 0)
                    _target.Damage(1);
                if(_projectile.DamagePoints > 0)
                    _projectile.Damage(1);
                _reward.IncreaseReward(1); 
                _projectileSpawner.SpawnFlyingProjectiles();  
                yield return new WaitForSeconds((float)waitSeconds);
            }
            AdvanceSceneState();                    
            _onePerIsRunning = false;
        }
        
        void AdvanceStateIfDecayTargetBeSpent(IDamageable decayTarget, BigInteger damage)
        {                    
            var delta = decayTarget.DamagePoints - damage;
            if(delta <= 0)
                AdvanceSceneState();   
        }     
        
        void DealDamage(IDamageable decayTarget, BigInteger damage)
        {            
            damage = (damage >= 1) ? damage : 1;   
            damage = (damage < decayTarget.DamagePoints) ? damage : decayTarget.DamagePoints;  
            if(_target.DamagePoints > 0)
                _target.Damage((damage <= _target.DamagePoints) ? damage : _target.DamagePoints);
            _projectile.Damage(damage);   
            _reward.IncreaseReward(damage); 
        }
        
        BigInteger GetFrameCountExponentially(IDamageable decayTarget)
        {
            var damage = _damageCalculator.GetDeltaForGivenTime(decayTarget.DamagePoints, Time.deltaTime);
            return damage;
        }
        
        BigInteger GetFrameCountConstantSpeed(IDamageable decayTarget, double speed)
        {
            var damage = GetConstantDamage(speed); 
            return damage;       
        }        
        
        
        BigInteger GetConstantDamage(double speed) => new BigInteger(Time.deltaTime*speed);
    }
}
