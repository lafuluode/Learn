using Game.Framework.Audio;
using Game.Framework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Core
{
    public class AudioSystem : IGameSystem
    {
        private AudioSource bgmSource;
        private AudioSource sfxSource;

        private float defaultBGMVolume = 0.6f;
        private float defaultSFXVolume = 1f;
        private bool defaultMuted = false;


        private IAudioService audioService;

        public int Priority => 20;


        public AudioSystem(AudioHost audioHost)
        {
            bgmSource = audioHost.bgmSource;
            sfxSource = audioHost.sfxSource;
        }
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