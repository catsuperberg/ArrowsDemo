using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.GameState
{    
    public class RunthroughUI : MonoBehaviour, UnityEngine.InputSystem.DefaultInputActions.IUIActions
    {
        [SerializeField]
        GameObject GameplayHUD;
        [SerializeField]
        GameObject FinishingScreen;
        [SerializeField]
        GameObject PreAdTease;   
        [SerializeField]
        GameObject PauseScreen;            
        
        public GameObject ActiveScreen {get; private set;}
        public event EventHandler OnPauseRequest;
        public event EventHandler OnPauseUnpauseRequest;
        public event EventHandler OnContinueGameplayRequest;
               
        void Awake()
        {
            SubscribeToDefaultInputActions();
            HideAllUIGameObjects();
        }
        
        void SubscribeToDefaultInputActions()
        {            
            var actions = new DefaultInputActions();
            actions.UI.Enable();
            actions.UI.SetCallbacks(this);
        }
        
        // HACK UI screens are active by default to create all the scripts    
        void HideAllUIGameObjects()     
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                FinishingScreen.SetActive(false);
                PreAdTease.SetActive(false);
                PauseScreen.SetActive(false);
                });             
        }
        
        public void RequestPause()
        {
            OnPauseRequest?.Invoke(this, EventArgs.Empty);
        }
        
        public void RequestPauseOrUnpause()
        {
            OnPauseUnpauseRequest?.Invoke(this, EventArgs.Empty);
        }
                        
        public void RequestContinueGameplay()
        {            
            OnContinueGameplayRequest?.Invoke(this, EventArgs.Empty);
        }
        
        public void SwithchToGameplay()
        {
            SwitchToScreen(GameplayHUD);  
        }
        
        public void SwithchToPause()
        {      
            SwitchToScreen(PauseScreen);  
        }
        
        public void SwithchToFinishingScene()
        {      
            SwitchToScreen(FinishingScreen);  
        }
        
        public void SwithchToPreAdTease()
        {      
            SwitchToScreen(PreAdTease);  
        }
        
        void SwitchToScreen(GameObject screen, bool forceUpdateIfNotChanged = false)
        {            
            if(ActiveScreen == screen && forceUpdateIfNotChanged)
                return;
                
            HideActiveScreen();
            ActiveScreen = screen;
            ActiveScreen.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
        }
        
        void HideActiveScreen()
        {
            if(ActiveScreen != null)
                ActiveScreen.SetActive(false);
        }
                
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
            // SelectFirstButton();
        }
        
        void SelectFirstButton()
        {
            if(EventSystem.current.currentSelectedGameObject != null)
                return;           
            
            var FocusButton = ActiveScreen.GetComponentInChildren<Button>();
            if(FocusButton != null)
                FocusButton.Select();
        }
        
        public void OnCancel(InputAction.CallbackContext context)
        {
            RequestPauseOrUnpause();
        }        
        
        public void OnClick(InputAction.CallbackContext context){}
        public void OnMiddleClick(InputAction.CallbackContext context){}        
        public void OnPoint(InputAction.CallbackContext context){}
        public void OnRightClick(InputAction.CallbackContext context){}
        public void OnScrollWheel(InputAction.CallbackContext context){}
        public void OnSubmit(InputAction.CallbackContext context){}
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context){}
        public void OnTrackedDevicePosition(InputAction.CallbackContext context){}
    }
}