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
        
        enum State
        {   
            HalfLifeCalculation,
            ConstantCalculation,
            Overkill,
            Blank
        }
        
        public event EventHandler OnFinished;
        
        double _halfLifeTime = 4;
        double _constantSpeedTime = 1;
        double _constantSpeed {get {return (double)_stopHalfLifeAt/_constantSpeedTime;}}
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
            _damageCalculator =  GetCalculator(_result);
            _state = State.HalfLifeCalculation;
        }
        
        void Update()
        {
            switch (_state)
            {
                case State.HalfLifeCalculation:
                    DecreaseCountWithHalfLife();
                    break;                    
                case State.ConstantCalculation:
                    DecreaseCountConstantly();
                    break;
            }
        } 
        
        HalfLifeCalculator GetCalculator(ResultType result)
        {
            switch (result)
            {
                case ResultType.Fail:
                case ResultType.Exact:
                     return new HalfLifeCalculator(_projectile.DamagePoints, _stopHalfLifeAt, _halfLifeTime);
                case ResultType.Overkill:
                     return new HalfLifeCalculator(_target.DamagePoints, _stopHalfLifeAt, _halfLifeTime);
                default:
                    return null;
            }
        }
        
        void DecreaseCountWithHalfLife()
        {
            BigInteger damage = new BigInteger(0);
            switch (_result)
            {
                case ResultType.Fail:
                case ResultType.Exact:                     
                    damage = _damageCalculator.CalculateDecayed(_projectile.DamagePoints, Time.deltaTime);
                    _target.Damage(damage);
                    _projectile.Damage(damage);
                    if(_projectile.DamagePoints < _stopHalfLifeAt)
                        _state = State.ConstantCalculation;
                    break;
                case ResultType.Overkill:                     
                    damage = _damageCalculator.CalculateDecayed(_target.DamagePoints, Time.deltaTime);
                    _target.Damage(damage);
                    _projectile.Damage(damage);
                    if(_target.DamagePoints < _stopHalfLifeAt)
                        _state = State.ConstantCalculation;
                    break;
            }
        }
        
        void DecreaseCountConstantly()
        {
            var damage = new BigInteger(Time.deltaTime*_constantSpeed);
            damage = (damage >= 1) ? damage : 1;
            BigInteger delta = new BigInteger(0);
            switch (_result)
            {
                case ResultType.Fail:
                case ResultType.Exact:
                    delta = _projectile.DamagePoints - damage;
                    if(delta <= 0)
                    {
                        damage = damage - delta;
                        _state = State.Blank;                  
                    } 
                    _target.Damage(damage);
                    _projectile.Damage(damage);           
                     break;
                case ResultType.Overkill:
                    delta = _target.DamagePoints - damage;
                    if(delta <= 0)
                    {
                        damage = damage - delta;   
                        _state = State.Overkill;                           
                    } 
                    _target.Damage(damage);
                    _projectile.Damage(damage);           
                     break;                    
            }
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
