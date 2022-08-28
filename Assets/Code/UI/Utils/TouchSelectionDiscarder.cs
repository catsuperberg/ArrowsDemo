using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{    
    public class TouchSelectionDiscarder : MonoBehaviour, Controls.ITouchMovementActions
    {
        Controls _gameplayControlls;
        
        void Awake()
        {
            EnableControlls();
        }
        
        void OnDestroy()
        {
            _gameplayControlls.TouchMovement.Disable();
            _gameplayControlls.TouchMovement.SetCallbacks(null);
        }
        
        void EnableControlls()
        {            
            _gameplayControlls = new Controls();
            _gameplayControlls.TouchMovement.Enable();
            _gameplayControlls.TouchMovement.SetCallbacks(this);
        }
        
        public void OnPrimaryContact(InputAction.CallbackContext context)
        {  
            if(context.performed)
                StartCoroutine(DeselectButtonAfter(0.2f));     
        }
        
        public void OnPrimaryPosition(InputAction.CallbackContext context)
        {
        }
        
        IEnumerator DeselectButtonAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);  
                
            EventSystem.current.SetSelectedGameObject(null);  
        }
    }
}
