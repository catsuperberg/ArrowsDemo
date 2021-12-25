using State;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UI_StateManager : MonoBehaviour
{
    [SerializeField]
    GameObject _startScreenMessage;
    [SerializeField]
    GameObject _LoadingScreen;
    [SerializeField]
    GameObject _GenerationIcon;
    [SerializeField]
    GameObject _PauseMenu;
    [SerializeField]
    GameObject _DebugMenu;
    
    GameObject _activeCanvas = null;
    HashSet<SubState> _subStates;
    List<GameObject> _activeSubCanvases = new List<GameObject>();
    
    [Inject]
    public void Construct(IStateChangeNotifier stateNotifier)
    {
        stateNotifier.OnStateChanged += OnStateNotification;
    }
    
    void OnStateNotification(object sender, StateEventArgs e)
    {
        Debug.LogWarning(e.State);
        if(Enum.IsDefined(typeof(AppState), e.State))
        {                
            switch (e.State)
            {
                case AppState.GameLaunch:
                    SwitchToScreen(_LoadingScreen);
                    break;
                case AppState.StartScreen:
                    SwitchToScreen(_startScreenMessage);
                    break;
                case AppState.GamePlay:
                    SwitchToScreen(null);
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
        
        List<GameObject> subCanvasesToShow = new List<GameObject>();
        
        if(e.SubStates.Any())
        {
            foreach(var subState in e.SubStates)
            {                
                Debug.Log(subState);
                switch (subState)
                {
                    case SubState.Generation:
                        subCanvasesToShow.Add(_GenerationIcon);
                        break;
                    case SubState.PauseMenu:
                        subCanvasesToShow.Add(_PauseMenu);
                        break;            
                }
            }
            Debug.Log("new Sub states in UI " + string.Join("", e.SubStates));
        }
        else
        {  
            Debug.Log("No Sub states");
        }
        
        List<GameObject> subCanvasesToDisable = _activeSubCanvases.Except(subCanvasesToShow).ToList();
        List<GameObject> subCanvasesToEnable = subCanvasesToShow.Except(_activeSubCanvases).ToList();
        if(subCanvasesToDisable.Any())
        {
            foreach(var canvas in subCanvasesToDisable)
                canvas.SetActive(false);
            _activeSubCanvases = _activeSubCanvases.Except(subCanvasesToDisable).ToList();
        }
        if(subCanvasesToEnable.Any())
        {
            foreach(var canvas in subCanvasesToEnable)
                canvas.SetActive(true);
            _activeSubCanvases.AddRange(subCanvasesToEnable);
        }
    }
    
    void SwitchToScreen(GameObject screen)
    {        
        if(_activeCanvas != screen)
        {
            if(_activeCanvas != null)
                _activeCanvas.SetActive(false);
            if(screen == null)
            {
                _activeCanvas = null;
                return;
            }
            _activeCanvas = screen;      
            _activeCanvas.SetActive(true);      
        }
        else if(_activeCanvas != null)
            if(!_activeCanvas.activeSelf)
                _activeCanvas.SetActive(true);
    }
}
