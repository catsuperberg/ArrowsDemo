using System;
using System.Collections.Generic;
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
        Ad _ad;       
                
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
            _runthrough.DestroyRun();     
            Destroy(_runthrough.gameObject);
            AdvanceState();
            ProcessCurrentState();
        }
        
        void StartPostRun()
        {            
            _postRun = _stateFactory.GetPostRun();
            _postRun.gameObject.transform.SetParent(this.transform);
            // AdvanceState();
            // ProcessCurrentState();
        }
        
        void StartAd()
        {            
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
            if(previousState == AppState.Runthrough)
                return true;
            else
                return false;
        }
    }
}