using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Framework.Core
{
    /// <summary>
    /// 资源管理接口，提供加载、实例化和释放资源的方法
    /// </summary>
    public interface IAssetService
    {
        Task<T> LoadAssetAsync<T>(string key) where T : Object;

        Task<GameObject> InstantiateAsync(string key, Transform parent = null);

        void ReleaseAsset(string key);

        void ReleaseInstance(GameObject obj);

        void ReleaseAll();
    }

    public class AddressableService : IAssetService
    {
        private readonly Dictionary<string, AsyncOperationHandle> loadedHandles = new();

        public async Task<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (loadedHandles.TryGetValue(key, out var cachedHandle))
            {
                return cachedHandle.Result as T;
            }
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"加载失败：{key}");
                return null;
            }

            loadedHandles[key] = handle;
            return handle.Result;
        }

        public async Task<GameObject> InstantiateAsync(string key, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(key, parent);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"加载失败: {key}");
                return null;
            }
            loadedHandles[key] = handle;
            return handle.Result;
        }

        public void ReleaseAsset(string key)
        {
            if (loadedHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                loadedHandles.Remove(key);
            }
        }
        public void ReleaseInstance(GameObject obj)
        {
            if (obj != null)
            {
                Addressables.ReleaseInstance(obj);
            }
        }
        public void ReleaseAll()
        {
            foreach (var handle in loadedHandles.Values)
            {
                Addressables.Release(handle);
            }
            loadedHandles.Clear();
        }
    }
}