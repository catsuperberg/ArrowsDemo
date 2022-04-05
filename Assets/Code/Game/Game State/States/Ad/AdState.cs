using System;
using System.Collections;
using UnityEngine;
using System.Timers;

namespace Game.GameState
{    
    public class AdState : MonoBehaviour
    {        
        public event EventHandler OnProceedToNextState;  
        private float AdDuration = 3000; 
        private Timer _timer;
        
        void Awake()
        {
            // ExecuteAfterTime(AdDuration, () => {OnProceedToNextState?.Invoke(this, EventArgs.Empty);});
            _timer = new Timer();
            _timer.Interval = AdDuration;
            _timer.Elapsed += FinishAd;
            _timer.Start();
        }
        
        void FinishAd(object sender, EventArgs args)
        {
            _timer.Stop();
            _timer.Elapsed -= FinishAd;
            _timer = null;
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    OnProceedToNextState?.Invoke(this, EventArgs.Empty);
                });
        }       
        
        IEnumerator ExecuteAfterTime(float time, Action task)
        {
            yield return new WaitForSeconds(time);
            
            task();
        }
    }
}