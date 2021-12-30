using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace State
{
    public class UIStateInputs : MonoBehaviour, IStateSignal
    {
        IStateSignal _gameState;
        
        [Inject]
        public void Construct(IStateSignal gameState)
        {
            _gameState = gameState;
        }
        
        void Awake()
        {
            
        }        
        
        public void SendStartGame()
        {
            ThrowIfNoGameStateObject();
            _gameState.SendStartGame();
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
    }
}
