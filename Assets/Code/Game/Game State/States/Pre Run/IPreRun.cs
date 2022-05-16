using Game.Gameplay.Realtime;
using System;
using UnityEngine;

namespace Game.GameState
{    
    public interface IPreRun
    {
        public GameObject GameObject {get;}
        public RunthroughContext CurrentRunthroughContext {get;}
        public event EventHandler OnProceedToNextState;   
    }
}