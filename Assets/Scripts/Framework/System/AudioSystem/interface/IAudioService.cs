using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Game.Framework.Audio
{
    /// <summary>
    /// 音频服务接口
    /// 
    /// 职责：
    /// 对外提供统一的音频播放接口
    /// 管理 BGM、SFX、音量、静音、暂停、停止等音频状态。
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// 当前是否静音
        /// 静音只表示音量为 0，不代表音频状态丢失。
        /// </summary>
        bool IsMuted { get; }

        /// <summary>
        /// 总音量，范围0-1
        /// 最终 BGM 音量 = MasterVolume * BGMVolume。
        /// 最终 SFX 音量 = MasterVolume * SFXVolume。
        /// </summary>
        float MasterVolume { get; }

        /// <summary>
        /// 当前BGM的音量，范围0-1
        /// </summary>
        float BGMVolume { get; }
        /// <summary>
        /// 当前音效音量，范围 0~1
        /// </summary>
        float SFXVolume { get; }
        /// <summary>
        /// 当前正在播放的 BGM 配置ID
        /// 如果当前没有人通过 audioId 播放 BGM ，可以为空
        /// </summary>
        string CurrentBGM { get; }
        /// <summary>
        /// 根据音频配置ID播放 BGM
        /// 
        /// audioId 不是 Addressables key，而是音频配置表里的 Id。
        /// AudioService 内部负责：
        /// 1. 查配置表
        /// 2. 获取资源 key
        /// 3. 通过资源系统加载 AudioClip
        /// 4. 播放 BGM
        /// </summary>
        /// <param name="audioId">音频配置 Id，例如 main_menu_bgm</param>
        /// <param name="restartIfSame">如果当前已经是同一个 BGM，是否重新播放</param>
        /// <returns></returns>
        Task PlayBGMAsync(string audioId, bool restartIfSame = false, bool loop = true);

        /// <summary>
        /// 根据音频配置ID播放一次性音效
        /// 
        /// audioId 不是 Addressables key，而是音频配置表里的 Id。
        /// </summary>
        /// <param name="audioId">音频配置 Id，例如 ui_click</param>
        /// <param name="volumeScale">本次播放的额外音量倍率</param>
        /// <returns></returns>
        Task PlaySFXAsync(string audioId, float volumeScale = 1f);

        /// <summary>
        /// 播放背景音乐
        /// 这个接口主要用于：
        /// 1. 临时测试
        /// 2. 其他系统已经持有 AudioClip 的情况
        /// 3. 还没接入配置表和资源系统之前的过渡阶段
        /// </summary>
        /// <param name="clip">要播放的 BGM 音频片段</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="restartIfSame">如果当前已经是同一个 BGM，是否重新播放</param>
        void PlayBGM(AudioClip clip, bool loop = true,bool restartIfSame = false);
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
        /// 停止所有正在播放的音效。
        /// 只有一个 SFX AudioSource 时，可以直接 Stop。
        /// 如果接入 AudioSourcePool，则停止所有池中的音效源。
        /// </summary>
        void StopAllSFX();
        /// <summary>
        /// 播放一次性音效
        /// 例如点击、错误、成功、胜利音效
        /// 底层通常使用AudioSource.PlayOneShot实现
        /// </summary>
        /// <param name="clip">这次要播放的音效片段</param>
        /// <param name="volumeScale">本次播放的音量倍率，范围0-1</param>
        void PlaySFX(AudioClip clip, float volumeScale = 1f);

        /// <summary>
        /// 设置总音量
        /// </summary>
        /// <param name="volume"></param>
        void SetMastersVolume(float volume);

        /// <summary>
        /// 停止所有音频，包括 BGM 和 SFX。
        /// 通常用于系统关闭、切账号、返回启动流程等场景。
        /// </summary>
        void StopAll();


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