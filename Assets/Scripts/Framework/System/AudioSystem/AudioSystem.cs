using Game.Framework.Audio;
using Game.Framework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Core
{
    public class AudioSystem : MonoBehaviour, IGameSystem
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Default Settings")]
        [SerializeField,Range(0f,1f)] private float defaultBGMVolume = 0.6f;
        [SerializeField,Range(0f,1f)] private float defaultSFXVolume = 1f;
        [SerializeField] private bool defaultMuted = false;

        private IAudioService audioService;

        public int Priority => 20;

        public void OnInit()
        {
            audioService = new AudioService(bgmSource, sfxSource);
            audioService.SetBGMVolume(defaultBGMVolume);
            audioService.SetSFXVolume(defaultSFXVolume);
            audioService.SetMuted(defaultMuted);

            ServiceLocator.Register<IAudioService>(audioService);
            Debug.Log("AudioSystem initialized.");
        }

        public void OnShutdown()
        {
            audioService?.StopBGM();
            audioService = null;

            ServiceLocator.Unregister<IAudioService>();
            Debug.Log("[AudioSystem] shutdown.");
        }
        
        
    }
}