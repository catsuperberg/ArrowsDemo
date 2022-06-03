using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.GameState
{    
    public class PreRunUI : MonoBehaviour, UnityEngine.InputSystem.DefaultInputActions.IUIActions
    {
        [SerializeField]
        GameObject StartScreen;
        [SerializeField]
        GameObject SettingsScreen;
        [SerializeField]
        GameObject LoadingScreen;   
         
        [SerializeField]
        private Animator PreRunAnimator; 
           
        
        public GameObject ActiveScreen {get; private set;}
        public event EventHandler OnStartRunthrough;
        bool firstStartScreenAnimation = true;
               
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
                StartScreen.SetActive(false);
                SettingsScreen.SetActive(false);
                // LoadingScreen.SetActive(false);
                });             
        }
        
        public void StartRunthrough()
        {
            OnStartRunthrough?.Invoke(this, EventArgs.Empty);
        }
        
        public void SwithchToStartScreen()
        {
            if(firstStartScreenAnimation)
            {
                PreRunAnimator.SetTrigger("LoadToStartScreen");
                firstStartScreenAnimation = false;
            }
            else
                PreRunAnimator.SetTrigger("SettingsToStart");                
            SwitchToScreen(StartScreen);  
        }
        
        public void SwithchToSettingsScreen()
        {      
            PreRunAnimator.SetTrigger("StartToSettings");
            SwitchToScreen(SettingsScreen);  
        }
        
        public void SwithchToLoadingScreen()
        {      
            SwitchToScreen(LoadingScreen);  
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
            SelectFirstButton();
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
            SwitchToScreen(StartScreen);
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