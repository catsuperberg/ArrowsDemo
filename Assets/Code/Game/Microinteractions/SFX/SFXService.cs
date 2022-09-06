using DataManagement;
using GameMath;
using System;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Game.Microinteracions
{
    public class SFXService : Configurable
    {        
        [StoredField("Sound", 0f, 1f)]
        public float SFXVolume {get; private set;} = 1f; 
        
        AudioSource _source;
        AudioMixerGroup _mixerGroup;
        
        AudioClip _affirmative = Resources.Load<AudioClip>("Sound Effects/Affirmative");
        AudioClip _negative = Resources.Load<AudioClip>("Sound Effects/Negative");
              
        public SFXService([Inject(Id = "settingsIngester")] IRegistryIngester registry,[Inject(Id = "SFX")] AudioSource source)
        {
            registry.Register(this, true, true);
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mixerGroup = _source.outputAudioMixerGroup; 
        }
        
        public void ExecuteEffect(SoundEffect effect)
        {
            switch (effect)
            {
                case SoundEffect.Affirmative:
                    AffirmativeSound();
                    break;
                case SoundEffect.Negative:
                    NegativeSound();
                    break;
            }
        }
        
        public void AffirmativeSound()
        {
            _source.PlayOneShot(_affirmative);
        } 
        
        public void NegativeSound()
        {                        
            _source.PlayOneShot(_negative);
        }
            
        internal override void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(SFXVolume):
                    SFXVolume = Convert.ToSingle(fieldValue);
                    SetMixerVolume();
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
        
        void SetMixerVolume()
        {
            _mixerGroup.audioMixer.SetFloat("VolumeSFX",  MathUtils.PartToNegativeDB(SFXVolume));
            _source.mute = SFXVolume <= 0;
        }
    }    
    
}