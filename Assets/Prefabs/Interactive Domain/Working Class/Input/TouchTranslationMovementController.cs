using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay
{    
    public class TouchTranslationMovementController : MonoBehaviour, Controls.ITouchMovementActions
    {              
        IMovable _movableObject;
        float _x_axisValue = 0;
        float _y_axisValue = 0;
        
        const float defaultCoefficient = 0.1f;
        public float _outputValuePerInput {get; private set;} = defaultCoefficient;  
        private bool _initialized = false;
        
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
            var gameplayControlls = new Controls();
            gameplayControlls.TouchMovement.Enable();
            gameplayControlls.TouchMovement.SetCallbacks(this);
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
            x_Delta *= _outputValuePerInput;
            y_Delta *= _outputValuePerInput;
            
            _movableObject.moveRight(x_Delta);
            _movableObject.moveUp(y_Delta);
            
            _x_axisValue = context.ReadValue<Vector2>().x;
            _y_axisValue = context.ReadValue<Vector2>().y;
        }
    }    
}