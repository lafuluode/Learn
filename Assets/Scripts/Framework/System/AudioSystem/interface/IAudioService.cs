using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Audio
{
    /// <summary>
    /// 音频服务接口
    /// 
    /// 职责：
    /// 支持BGM、音效、静音、暂停等基础能力
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// 当前是否静音
        /// </summary>
        bool IsMuted { get; }
        /// <summary>
        /// 当前BGM的音量，范围0-1
        /// </summary>
        float BGMVolume { get; }
        /// <summary>
        /// 当前音效音量，范围 0~1
        /// </summary>
        float SFXVolume { get; }
        /// <summary>
        /// 播放背景音乐
        /// 一般 BGM 使用单独的 AudioSource 循环播放
        /// 如果当前已经在播放其他 BGM，会直接替换
        /// </summary>
        /// <param name="clip">要播放的 BGM 音频片段</param>
        /// <param name="loop">是否循环播放</param>
        void PlayBGM(AudioClip clip, bool loop = true);
        /// <summary>
        /// 停止当前 BGM
        /// </summary>
        void StopBGM();
        /// <summary>
        /// 暂停当前 BGM
        /// </summary>
        void PauseBGM();
        /// <summary>
        /// 恢复当前 BGM
        /// </summary>
        void ResumeBGM();
        /// <summary>
        /// 播放一次性音效
        /// 例如点击、错误、成功、胜利音效
        /// 底层通常使用AudioSource.PlayOneShot实现
        /// </summary>
        /// <param name="clip">这次要播放的音效片段</param>
        /// <param name="volumeScale">本次播放的音量倍率，范围0-1</param>
        void PlaySFX(AudioClip clip, float volumeScale = 1f);
        /// <summary>
        /// 设置 BGM 音量
        /// </summary>
        /// <param name="volume"></param>
        void SetBGMVolume(float volume);
        /// <summary>
        /// 设置音效音量
        /// </summary>
        /// <param name="volume">音量，范围 0~1</param>
        void SetSFXVolume(float volume);
        /// <summary>
        /// 设置是否静音
        /// 静音后BGM和SFX都不应该发声
        /// </summary>
        /// <param name="isMuted">是否静音</param>
        void SetMuted(bool isMuted);
    }
}