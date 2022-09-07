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
        private float AdDuration = 5000; 
        private Timer _timer;
        
        UnityEngine.Audio.AudioMixer _musicMixer;
        float _oldMusicVolume;
        
        public void Initialize(AudioSource musicSource)
        {
            var source = musicSource ?? throw new ArgumentNullException(nameof(musicSource));
            _musicMixer = source.outputAudioMixerGroup.audioMixer;
        }
        
        void Awake()
        {
            // ExecuteAfterTime(AdDuration, () => {OnProceedToNextState?.Invoke(this, EventArgs.Empty);});
             UnityMainThreadDispatcher.Instance().Enqueue(() => {MuteEverithingButAD();});
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
                    SetAudioToPreviousState();
                    OnFinished?.Invoke(this, EventArgs.Empty); // HACK Hangs on pre run loading if OnFinished (reward applier) is invoked after OnProceedToNextState
                    OnProceedToNextState?.Invoke(this, EventArgs.Empty);});
        }     
        
        // HACK Unity audio system is super rudimentary, so for serius audio some custom system should be build
        void MuteEverithingButAD()
        {
            _musicMixer.GetFloat("VolumeMusic", out _oldMusicVolume);
            _musicMixer.SetFloat("VolumeMusic", -80);
        }
        
        void SetAudioToPreviousState()
        {
            _musicMixer.SetFloat("VolumeMusic", _oldMusicVolume);            
        }  
        
        IEnumerator ExecuteAfterTime(float time, Action task)
        {
            yield return new WaitForSeconds(time);
            
            task();
        }
    }
}