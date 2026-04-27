using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Framework.Core
{
    /// <summary>
    /// 资源管理接口，提供加载、实例化和释放资源的方法
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
        Task<long> GetDownloadSizeAsync(string key);
        /// <summary>
        /// 下载指定资源或资源组的所有依赖项，确保它们在加载时可用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DownloadDependenciesAsync(string key);
        
        /// <summary>
        /// 加载指定的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> LoadAssetAsync<T>(string key) where T : Object;

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