using System;
using System.Collections.Generic;
using Game.GameState.Context;
using UnityEngine;
using Zenject;

namespace Game.GameState
{    
    public class GameState : MonoBehaviour
    {           
        IAppStateFactory _stateFactory;
        AppState _state;
        AppState _previousState;
        List<AppState> _statesToGoThrough = new List<AppState>()
            {AppState.PreRun,
            AppState.Runthrough,
            AppState.PostRun};
        List<AppState>.Enumerator _stateEnumerator;
        
        PreRun _preRun;
        Runthrough _runthrough;
        PostRun _postRun;
        AdState _ad;       
        
        PostRunContext _postRunContext = null;
                        
        [Inject]
        public void Construct(IAppStateFactory stateFactory)
        {                        
            if(stateFactory == null)
                throw new ArgumentNullException("IAppStateFactory not provided to " + this.GetType().Name);
            
            _stateFactory = stateFactory;
            _stateEnumerator = _statesToGoThrough.GetEnumerator();            
        }
        
        void Start()
        {
            AdvanceState();
            ProcessCurrentState();
        }
        
        void ProcessCurrentState()
        {
            switch(_state)
            {
                case AppState.PreRun:
                    StartPreRun();
                    break;
                case AppState.Runthrough:
                    StartRunthrough();
                    break;
                case AppState.PostRun:
                    StartPostRun();
                    break;
                case AppState.Ad:
                    StartAd();
                    break;
            } 
            _previousState = _state;
        }
        
        void StartPreRun()
        {            
            _preRun = _stateFactory.GetPreRun();
            _preRun.gameObject.transform.SetParent(this.transform);
            _preRun.OnProceedToNextState += PreRunCalledStartRunthrough;
        }
        
        void PreRunCalledStartRunthrough(object caller, EventArgs args)
        {
            _preRun.OnProceedToNextState -= PreRunCalledStartRunthrough;
            AdvanceState();
            ProcessCurrentState();
        }
        
        void StartRunthrough()
        {
            _runthrough = _stateFactory.GetRunthrough(_preRun.NextRunthroughContext);
            _runthrough.gameObject.transform.SetParent(this.transform);
            Destroy(_preRun.gameObject);
            _preRun = null;
            _runthrough.OnProceedToNextState += RunthroughFinished;
            _runthrough.StartRun();
        }
        
        void RunthroughFinished(object caller, EventArgs args)
        {
            _runthrough.OnProceedToNextState -= PreRunCalledStartRunthrough;  
            AdvanceState();
            ProcessCurrentState();
        }
        
        void StartPostRun()
        {            
            _postRun = _stateFactory.GetPostRun(_runthrough.FinishingContext);
            _runthrough.DestroyRun();     
            Destroy(_runthrough.gameObject);
            _runthrough = null;
            _postRun.gameObject.transform.SetParent(this.transform);
            _postRun.OnProceedToNextState += PostRunFinished;
        }
        
        void PostRunFinished(object caller, EventArgs args)
        {            
            _postRun.OnProceedToNextState -= PostRunFinished;
            _postRunContext = _postRun.Context;
            if(!_postRun.Context.ShowAdBeforeApplyingReward)            
            {
                _postRun.AddReward();
                Destroy(_postRun.gameObject);         
                _postRun = null;
            }
            AdvanceState();
            ProcessCurrentState();
        }
        
        void StartAd()
        {         
            _ad = _stateFactory.GetAd();
            
            _postRun.SubscribeActualReward(_ad);
            Destroy(_postRun.gameObject);            
            _postRun = null;
             
            _ad.gameObject.transform.SetParent(this.transform);
            _ad.OnProceedToNextState += AdFinished;
        }
        
        void AdFinished(object caller, EventArgs args)
        {                                    
            _ad.OnProceedToNextState -= AdFinished;
            Destroy(_ad.gameObject);    
            _ad = null;
            AdvanceState();
            ProcessCurrentState();
        }
        
        void AdvanceState()
        {     
            if(DecideOnShowingAd(_state))
            {
                _state = AppState.Ad;
                return;
            }
            
            var hasNext =_stateEnumerator.MoveNext();
            if(!hasNext)
            {
                _stateEnumerator = _statesToGoThrough.GetEnumerator();  
                _stateEnumerator.MoveNext();
            }            
            _state = _stateEnumerator.Current; 
        }
        
        bool DecideOnShowingAd(AppState previousState)
        {
            if(previousState == AppState.PostRun)
                return _postRunContext.ShowAdBeforeApplyingReward;
            else
                return false;
        }
    }
}