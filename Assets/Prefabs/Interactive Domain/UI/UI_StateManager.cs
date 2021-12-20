using State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UI_StateManager : MonoBehaviour
{
    [SerializeField]
    GameObject _startScreenMessage;
    
    GameObject _activeCanvas;
    
    [Inject]
    public void Construct(IStateChangeNotifier stateNotifier)
    {
        stateNotifier.OnStateChanged += OnStateNotification;
    }
    
    void OnStateNotification(object sender, StateEventArgs e)
    {
        if(Enum.IsDefined(typeof(AppState), e.State))
        {
            if(_activeCanvas != null)
                _activeCanvas.SetActive(false);
                
            switch (e.State)
            {
                case AppState.StartScreen:
                    DisplayStartScreenMessage();
                    break;
                
                case AppState.GamePlay:
                    
                    break;
                case AppState.FinishingCutscene:
                    
                    break;
                case AppState.PreAdTease:
                    
                    break;  
                case AppState.Menu:
                    
                    break;    
                case AppState.Blank:
                    
                    break;               
            }
        }        
    }
    
    void DisplayStartScreenMessage()
    {
        _activeCanvas = _startScreenMessage;
        _activeCanvas.SetActive(true);
    }
}
