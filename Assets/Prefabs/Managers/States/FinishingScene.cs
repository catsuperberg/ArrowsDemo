using GamePlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Utils;

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
        IDamageable _projectile;
        IDamageable _target;
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
                
        public void StartScene(IDamageable projectile, IDamageable target)
        {     
             if(projectile == null)
                throw new System.Exception("IProjectileObject isn't provided to FinishingScene");
             if(target == null)
                throw new System.Exception("IDamageable isn't provided to FinishingScene");    
            
            
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
            }
        } 
        
        void SetValuesToDecay()
        {
            switch (_result)
            {
                case ResultType.Fail:
                case ResultType.Exact:
                    _smallerDamageable = _projectile;
                    _largerDamageable = _target;
                    break;
                case ResultType.Overkill:
                    _smallerDamageable = _target;
                    _largerDamageable = _projectile;
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
