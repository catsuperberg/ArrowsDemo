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
                    
                    break;
                case AppState.Ad:
                    
                    break;
            } 
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
            GameObject.Destroy(_preRun.gameObject);
            _preRun = null;
            _runthrough.StartRun();
        }
        
        void AdvanceState()
        {     
            if(DecideOnShowingAd(_stateEnumerator.Current))
            {
                _state = AppState.Ad;
                return;
            }
            
            _stateEnumerator.MoveNext();
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