using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Framework.Core
{
    /// <summary>
    /// 资源管理接口，提供加载、实例化、释放和场景加载
    /// 不负责资源版本检查、catalog 更新和下载
    /// 不关心底层实现
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// 初始化资源服务
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();
        /// <summary>
        /// 获取指定资源或资源组的下载大小，单位为字节
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        
        Task<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object;

        /// <summary>
        /// 加载并实例化指定的资源，返回实例化后的GameObject
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        Task<GameObject> InstantiateAsync(string key, Transform parent = null);
        /// <summary>
        /// 加载指定场景。
        /// </summary>
        Task LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single);
        /// <summary>
        /// 卸载指定场景。
        /// </summary>
        Task UnloadSceneAsync(string key);
        /// <summary>
        /// 释放指定资源。
        /// </summary>
        void ReleaseAsset(string key);
        /// <summary>
        /// 释放指定实例对象。
        /// </summary>
        void ReleaseInstance(GameObject obj);
        /// <summary>
        /// 释放当前服务持有的所有资源和实例对象,不会卸载场景。
        /// </summary>
        void ReleaseAll();
    }
}