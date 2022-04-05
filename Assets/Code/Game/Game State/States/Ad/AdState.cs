using System;
using System.Collections;
using UnityEngine;
using System.Timers;

namespace Game.GameState
{    
    public class AdState : MonoBehaviour, IFinishNotification
    {        
        public event EventHandler OnProceedToNextState; 
        public event EventHandler OnFinished;   
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
                    OnFinished?.Invoke(this, EventArgs.Empty); // HACK Hangs on pre run loading if OnFinished (reward applier) is invoked after OnProceedToNextState
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