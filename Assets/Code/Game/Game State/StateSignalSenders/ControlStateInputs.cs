using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.GameState.StateSignalSenders
{
    public class ControlStateInputs : MonoBehaviour, IStateSignal, Controls.IStateActions
    {
        IStateSignal _gameState;
        
        [Inject]
        public void Construct(IStateSignal gameState)
        {
            _gameState = gameState;
        }
        
        void Awake()
        {
            var stateControls = new Controls();
            stateControls.State.Enable();
            stateControls.State.SetCallbacks(this);
        }        
        
        public void SendStartGame()
        {
            ThrowIfNoGameStateObject();
            _gameState.SendStartGame();
        }
        
        public void SendUpgradeShop()
        {
            ThrowIfNoGameStateObject();
            _gameState.SendUpgradeShop();
        }
        
        public void SendPauseMenu()
        {
            ThrowIfNoGameStateObject();
            _gameState.SendPauseMenu();            
        }
        
        public void SendDebugMenu()
        {
            ThrowIfNoGameStateObject();
            _gameState.SendDebugMenu();              
        }
        
        public void SendPreviousState()
        {
            ThrowIfNoGameStateObject();
            _gameState.SendPreviousState();  
        }
        
        void ThrowIfNoGameStateObject()
        {
            if(_gameState == null)
                throw new System.Exception("gameState isn't provided to ControlStateInputs");
        }
        
        public void OnStartGame(InputAction.CallbackContext context)
        {
            if(context.performed)
                SendStartGame();
        }
        
        public void OnPauseMenu(InputAction.CallbackContext context)
        {
            if(context.performed)
                SendPauseMenu();
        }
        
        public void OnDebugMenu(InputAction.CallbackContext context)
        {
            if(context.performed)
                SendDebugMenu();
        }
        
        public void OnBack(InputAction.CallbackContext context)
        {
            if(context.performed)
                SendPreviousState();
        }
    }
}
