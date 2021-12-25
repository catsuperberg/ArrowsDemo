using GamePlay;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Utils;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace State
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
        IDamageableWithTransforms _projectile;
        IDamageableWithTransforms _target;
        IDamageable _smallerDamageable;
        IDamageable _largerDamageable;
        
        enum State
        {   
            HalfLifeCalculation,
            ConstantCalculation,
            Overkill,
            Finished,
            Blank
        }
        
        public event EventHandler OnFinished;
        
        double _halfLifeTime = 4;
        double _constantSpeedTime = 1;
        double _finishingSpeed;
        double _overkillSpeed;
        BigInteger _stopHalfLifeAt = new BigInteger(500);
        ResultType _result = ResultType.Blank;
        State _state = State.Blank;
        HalfLifeCalculator _damageCalculator;
        
        int _maxAnimatedProjectiles = 300;
        List<GameObject> _animatedProjectiles = new List<GameObject>();
                
        public void StartScene(IDamageableWithTransforms projectile, IDamageableWithTransforms target)
        {     
             if(projectile == null)
                throw new System.Exception("IDamageableWithTransforms 'projectile' isn't provided to FinishingScene");
             if(target == null)
                throw new System.Exception("IDamageableWithTransforms 'target' isn't provided to FinishingScene");    
            
            
            _projectile = projectile;
            _target = target;
            _result = CheckResult();               
            SetValuesToDecay(); 
            _finishingSpeed = (double)_stopHalfLifeAt/_constantSpeedTime;
            _overkillSpeed = (_result == ResultType.Overkill) ? (double)(_projectile.DamagePoints - _target.DamagePoints)/_constantSpeedTime : 0; 
            _damageCalculator =  new HalfLifeCalculator(_smallerDamageable.DamagePoints, _stopHalfLifeAt, _halfLifeTime);
            _state = State.HalfLifeCalculation;
        }
        
        void Update()
        {
            switch (_state)
            {
                case State.HalfLifeCalculation:
                    DecreaseCountWithHalfLife(_smallerDamageable);
                    break;                    
                case State.ConstantCalculation:                    
                    DecreaseCountConstantly(_smallerDamageable, _finishingSpeed);
                    break;  
                case State.Overkill:
                    DecreaseCountConstantly(_largerDamageable, _overkillSpeed);                  
                    break;
                case State.Finished:
                    OnFinished?.Invoke(this, EventArgs.Empty);
                    break;
            }            
            SpawnFlyingProjectiles();
        } 
        
        Transform _randomProjectileTransform
        {
            get
            {
                var projectileIndex = Random.Range(0, _projectile.ChildrenTransforms.Count);
                return _projectile.ChildrenTransforms[projectileIndex];
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
                var tempObj = new GameObject("temp object, for transform only (selecting random target)");                
                tempObj.transform.position = tempTransform.position;
                tempObj.transform.rotation = Quaternion.LookRotation(Vector3.down);
                Transform anotherTransform = tempObj.transform;
                Destroy(tempObj);
                return anotherTransform;
            }
        }
        
        void SpawnFlyingProjectiles()
        {
            var projectileCount = (_animatedProjectiles.Any()) ? _animatedProjectiles.Count : 0;
            if(projectileCount <= _maxAnimatedProjectiles && Random.Range(1, 20) > 3)
            {
                var moverGameObject = new GameObject("Ballistic mover");
                var startTransform = _randomProjectileTransform;
                moverGameObject.transform.position = startTransform.position;
                moverGameObject.transform.rotation = startTransform.rotation;                
                var mover = moverGameObject.AddComponent<BallisticMover>();
                var projectile = Instantiate((_projectile as IProjectileObject).ProjectilePrefab, Vector3.zero, Quaternion.identity);
                mover.initialize(startTransform, _randomTargetTransform, 0.85f);
                mover.StartMover(200f);
                projectile.transform.SetParent(moverGameObject.transform, false);
                _animatedProjectiles.Add(moverGameObject);                
            }
            // if decided add arrow
                // select source arrow transform
                // select target transform
                // create arrow
                // create balistic mover and attach arrow to it
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
        
        void DecreaseCountWithHalfLife(IDamageable decayTarget)
        {
            var damage = _damageCalculator.CalculateDecayed(decayTarget.DamagePoints, Time.deltaTime);
            if(_target.DamagePoints > 0)
                _target.Damage(damage);
            _projectile.Damage(damage);
            if(decayTarget.DamagePoints < _stopHalfLifeAt)
                _state = State.ConstantCalculation;
        }
        
        void DecreaseCountConstantly(IDamageable decayTarget, double speed)
        {
            var damage = new BigInteger(Time.deltaTime*speed);
            damage = (damage >= 1) ? damage : 1;            
            var delta = decayTarget.DamagePoints - damage;
            if(delta <= 0)
            {
                damage += delta;
                _state = (_state != State.Overkill && _result == ResultType.Overkill) ? State.Overkill : State.Finished;                  
            } 
            if(_target.DamagePoints > 0)
                _target.Damage(damage);
            _projectile.Damage(damage);             
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
    }
}
