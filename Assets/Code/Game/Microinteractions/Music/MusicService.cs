using DataManagement;
using System;
using System.Timers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;
using GameMath;

namespace Game.Microinteracions
{
    public class MusicService : Configurable
    {        
        [StoredField("Music", 0f, 1f)]
        public float MusicVolume {get; private set;} = 0f; 
        
        const string MusicClipsResourceFolder = "Music";
        List<string> MusicFiles = Resources.LoadAll<AudioClip>(MusicClipsResourceFolder)
                                        .Where(entry => entry != null)
                                        .Select(entry => Path.Combine(MusicClipsResourceFolder, entry.name))
                                        .ToList();                                        
        AudioSource _source;
        AudioMixerGroup _mixerGroup;
        Timer _currentTrackTimer;
                
        public MusicService([Inject(Id = "settingsIngester")] IRegistryIngester registry,[Inject(Id = "Music")] AudioSource source)
        {
            registry.Register(this, true, true);
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mixerGroup = _source.outputAudioMixerGroup; 
            StartNewBackgroundMusicTrackLooped();
        }
        
        void StartNewBackgroundMusicTrackLooped()
        {
            var newTrack = Resources.Load<AudioClip>(MusicFiles[GlobalRandom.RandomInt(0, MusicFiles.Count)]);
            _currentTrackTimer = new Timer(MathUtils.SecondsToMs(newTrack.length));
            _currentTrackTimer.Enabled = true ;
            _source.clip = newTrack;
            _source.Play();
            _currentTrackTimer.Elapsed += TrackFinished;
        }
        
        void TrackFinished(object caller, EventArgs arguments)
        {
            _currentTrackTimer.Elapsed -= TrackFinished;
            _currentTrackTimer.Dispose();  
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartNewBackgroundMusicTrackLooped();});            
        }
            
        internal override void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(MusicVolume):
                    MusicVolume = Convert.ToSingle(fieldValue);
                    SetMixerVolume();
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
        
        void SetMixerVolume()
        {
            _mixerGroup.audioMixer.SetFloat("VolumeMusic", MathUtils.PartToNegativeDB(MusicVolume));
            _source.mute = MusicVolume <= 0;
        }
    }        
}