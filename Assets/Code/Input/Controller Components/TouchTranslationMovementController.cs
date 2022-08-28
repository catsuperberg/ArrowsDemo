using DataManagement;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input.ControllerComponents
{    
    public class TouchTranslationMovementController : MonoBehaviour, Controls.ITouchMovementActions, IConfigurable
    {        
        IMovable _movableObject;
        Controls _gameplayControls;
           
        [StoredField("Sensitivity", 3, 15)]
        public float Sensitivity {get; private set;} = 8;        
                 
        public event EventHandler OnUpdated;
        
        float _x_axisValue = 0;
        float _y_axisValue = 0;
        float _x_delta = 0;
        float _y_delta = 0;
        float _dpi = 130;
        private bool _controllsEnabled = false;
        
        public void Initialize(IRegistryIngester registry)
        {
            registry.Register(this, true, true);   
        }
        
        public void SetControlsEnabled(bool state)
        {               
            if(_controllsEnabled == state)
            {
                Debug.LogWarning("Trying to change controls state to same state as active");
                return;
            }
            _controllsEnabled = state;
            UpdateControllsActive();
        }
        
        void UpdateControllsActive()
        {
            if(_controllsEnabled)
                EnableControls();
            else
                DisableControls();
        }
        
        void Awake()
        {
            var dpi = Screen.dpi;
            if(dpi != 0)
                _dpi = dpi;
            
            _movableObject = gameObject.GetComponent<IMovable>();
        }
        
        void EnableControls()
        {            
            _gameplayControls = new Controls();
            _gameplayControls.TouchMovement.Enable();
            _gameplayControls.TouchMovement.SetCallbacks(this);
        }
        
        void DisableControls()
        {
            _gameplayControls.TouchMovement.Disable();
            _gameplayControls.TouchMovement.SetCallbacks(null);
            _gameplayControls = null;
        }
        
        void OnDestroy()
        {
            if(_gameplayControls != null)
                DisableControls();
        }
        
        void Update()
        {      
            if(_movableObject == null || Time.timeScale == 0)
                return;
                
            _movableObject.moveRight(_x_delta);
            _movableObject.moveUp(_y_delta);
            _x_delta = 0;
            _y_delta = 0;
        }
        
        public void OnPrimaryContact(InputAction.CallbackContext context)
        {
            var currentPositon = context.ReadValue<Vector2>();
            var currentPositonInch = currentPositon/_dpi;
            if(context.performed)
            {
                _x_axisValue = currentPositonInch.x;
                _y_axisValue = currentPositonInch.y;                
            }
        }
        
        public void OnPrimaryPosition(InputAction.CallbackContext context)
        {
            var currentPositon = context.ReadValue<Vector2>();
            var currentPositonInch = currentPositon/_dpi;
            if(context.performed)
            {
                var x_Delta = currentPositonInch.x - _x_axisValue;
                var y_Delta = currentPositonInch.y - _y_axisValue;
                if(x_Delta != 0 || y_Delta != 0)
                {
                    _x_delta = Sensitivity * x_Delta;
                    _y_delta = Sensitivity * y_Delta;                
                }
                
                
                _x_axisValue = currentPositonInch.x;
                _y_axisValue = currentPositonInch.y;     
            }
        }
        
        public void UpdateField(string fieldName, string fieldValue)
        {            
            SetFieldValue(fieldName, fieldValue);        
            
            OnUpdated?.Invoke(this, EventArgs.Empty);   
        }
        
        public void UpdateFields(List<(string fieldName, string fieldValue)> updatedValues)
        {            
            if(updatedValues.Count == 0)
                throw new System.Exception("No field data in array provided to UpdateFields function of class: " + this.GetType().Name);
            
            foreach(var fieldData in updatedValues)
                SetFieldValue(fieldData.fieldName, fieldData.fieldValue);       
                
            OnUpdated?.Invoke(this, EventArgs.Empty);    
        }
        
        void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(Sensitivity):
                    Sensitivity = Convert.ToSingle(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }    
}