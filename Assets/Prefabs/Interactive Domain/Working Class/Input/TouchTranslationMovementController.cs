using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay
{    
    public class TouchTranslationMovementController : MonoBehaviour, Controls.ITouchMovementActions, ISensitivitySetable
    {              
        IMovable _movableObject;
        float _x_axisValue = 0;
        float _y_axisValue = 0;
        float _x_delta = 0;
        float _y_delta = 0;
        
        const float defaultCoefficient = 8f;
        float _dpi = 130;
        public float _outputValuePerInput {get; private set;} = defaultCoefficient;  
        private bool _initialized = false;
        Controls _gameplayControlls;
        
        public void Init(float outputValuePerInput = defaultCoefficient)
        {
            if(!_initialized)
            {
                _outputValuePerInput = outputValuePerInput;            
                EnableControlls();
                _initialized = true;
            }
        }
        
        void Awake()
        {
            var dpi = Screen.dpi;
            if(dpi != 0)
                _dpi = dpi;
            
            _movableObject = gameObject.GetComponent<IMovable>();
            if(!_initialized)
                EnableControlls();
        }
        
        void EnableControlls()
        {            
            _gameplayControlls = new Controls();
            _gameplayControlls.TouchMovement.Enable();
            _gameplayControlls.TouchMovement.SetCallbacks(this);
        }
        
        // FIXME Should probably unsubscribe on destroy
        
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
                    _x_delta = _outputValuePerInput * x_Delta;
                    _y_delta = _outputValuePerInput * y_Delta;                
                }
                
                
                _x_axisValue = currentPositonInch.x;
                _y_axisValue = currentPositonInch.y;     
            }
        }
        
        public void UpdateSensitivity(float sensitivity)
        {
            Debug.Log("UpdateSensitivity() called");
            _outputValuePerInput = sensitivity;
        }
    }    
}