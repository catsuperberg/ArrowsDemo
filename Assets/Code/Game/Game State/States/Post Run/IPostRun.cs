using Game.GameState.Context;
using System;
using UnityEngine;

namespace Game.GameState
{    
    public interface IPostRun
    {
        public GameObject GO {get;}        
        public PostRunContext Context {get;}
        public event EventHandler OnProceedToNextState;   
    }
}