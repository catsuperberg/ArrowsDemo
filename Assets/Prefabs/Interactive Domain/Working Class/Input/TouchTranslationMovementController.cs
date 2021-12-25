using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay
{    
    public class TouchTranslationMovementController : MonoBehaviour, Controls.ITouchMovementActions
    {              
        IMovable _movableObject;
        float _x_axisValue = 0;
        float _y_axisValue = 0;
        float _x_delta = 0;
        float _y_delta = 0;
        
        const float defaultCoefficient = 0.03f;
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
            if(_movableObject != null)
            {      
                _movableObject.moveRight(_x_delta);
                _movableObject.moveUp(_y_delta);
            }
        }
        
        public void OnPrimaryContact(InputAction.CallbackContext context)
        {
            // _x_axisValue = context.ReadValue<Vector2>().x;
            // _y_axisValue = context.ReadValue<Vector2>().y;
        }
        
        public void OnPrimaryPosition(InputAction.CallbackContext context)
        {
            var x_Delta = context.ReadValue<Vector2>().x - _x_axisValue;
            var y_Delta = context.ReadValue<Vector2>().y - _y_axisValue;
            if(x_Delta != 0 || y_Delta != 0)
            {
                _x_delta = _outputValuePerInput * x_Delta;
                _y_delta = _outputValuePerInput * y_Delta;                
            }
            
            
            _x_axisValue = context.ReadValue<Vector2>().x;
            _y_axisValue = context.ReadValue<Vector2>().y;     
        }
    }    
}