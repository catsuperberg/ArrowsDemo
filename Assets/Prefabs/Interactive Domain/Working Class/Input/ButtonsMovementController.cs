using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay
{    
    public class ButtonsMovementController : MonoBehaviour, Controls.IMovementActions
    {              
        IMovable _movableObject;
        bool _x_axisActive = false;
        bool _y_axisActive = false;
        float _x_axisValue = 0;
        float _y_axisValue = 0;
        
        const float defaultSpeed = 25;
        public float _speed {get; private set;} = defaultSpeed;  
        private bool _initialized = false;
        
        public void Init(float speed = defaultSpeed)
        {
            if(!_initialized)
            {
                _speed = speed;            
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
            gameplayControlls.Movement.Enable();
            gameplayControlls.Movement.SetCallbacks(this);
        }
        
        void Update()
        {
            if(_movableObject != null)
            {
                if(_x_axisActive)
                    _movableObject.moveRight(_x_axisValue*_speed*Time.deltaTime);
                if(_y_axisActive)
                    _movableObject.moveUp(_y_axisValue*_speed*Time.deltaTime);
            }
        }
                
        public void OnX_axis(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                _x_axisActive = true;
                _x_axisValue = context.ReadValue<float>();  
            }
            if(context.canceled)
              _x_axisActive = false;  
        }
        
        public void OnY_axis(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                _y_axisActive = true;
                _y_axisValue = context.ReadValue<float>();                
            }
            if(context.canceled)
              _y_axisActive = false;  
        }
    }    
}