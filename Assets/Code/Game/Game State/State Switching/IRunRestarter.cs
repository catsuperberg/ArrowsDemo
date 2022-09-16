using Game.GameState.Context;
using System;
using UnityEngine;

namespace Game.GameState
{    
    public interface IRunRestarter
    {      
        public PostRunContext Context {get;}
        public event EventHandler OnProceedToRestart;   
    }
}