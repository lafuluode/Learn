using Game.Framework.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Audio
{
    /// <summary>
    /// 音频服务实现
    /// 
    /// 它负责真正操作 Unity 的 AudioSource
    /// 其他系统不应直接访问AudioSource，而是通过 IAudioService 来控制音频播放
    /// </summary>
    public class AudioService : IAudioService
    {
        private readonly AudioSource bgmSource;
        private readonly AudioSource sfxSource;

        private float bgmVolume = 1f;
        private float sfxVolume = 1f;
        private bool isMuted = false;

        public bool IsMuted => isMuted;
        public float BGMVolume => bgmVolume;
        public float SFXVolume => sfxVolume;

        public AudioService(AudioSource bgmSource,AudioSource sfxSource)
        {
            this.bgmSource = bgmSource;
            this.sfxSource = sfxSource;

            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            if(bgmSource != null)
            {
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                bgmSource.volume = bgmVolume;
            }
            if(sfxSource != null)
            {
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume;
            }
        }
        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if(clip == null || bgmSource == null) return;
            
            if(isMuted) return;
            // 如果当前正在播放同一个BGM，就不需要播放了
            if (bgmSource.clip==clip&& bgmSource.isPlaying) return;
            
            
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
        public void StopBGM()
        {
            if (bgmSource == null) return;
            bgmSource.Stop();
            bgmSource.clip = null;
        }
        public void PauseBGM()
        {
            if (bgmSource == null) return;
            bgmSource.Pause();
        }
        public void ResumeBGM()
        {
            if(bgmSource == null) return;   

            if(!isMuted) bgmSource.UnPause();
        }
        public void PlaySFX(AudioClip clip, float volumeScale = 1)
        {
            if (clip == null || sfxSource == null) return;
            if (isMuted) return;

            float finalVolume = Mathf.Clamp01(sfxVolume * volumeScale);
            sfxSource.PlayOneShot(clip, finalVolume);
        }
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);

            if(bgmSource != null)
            {
                bgmSource.volume = isMuted ? 0 : bgmVolume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if(sfxSource != null)
            {
                sfxSource.volume = isMuted ? 0 : sfxVolume;
            }
        }
        public void SetMuted(bool isMuted)
        {
            this.isMuted = isMuted;

            if(bgmSource != null)
            {
                bgmSource.volume = isMuted ? 0 : bgmVolume;
            }
            if(sfxSource != null)
            {
                sfxSource.volume = isMuted ? 0 : sfxVolume;
            }
        }   

    }
}