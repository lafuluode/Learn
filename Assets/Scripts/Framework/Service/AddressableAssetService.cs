using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Framework.Core
{

    public class AddressableAssetService : IAssetService
    {
        private readonly AddressableInitializer initializer;
        private readonly Dictionary<string, AsyncOperationHandle> loadedAssetHandles = new();
        private readonly Dictionary<string,AsyncOperationHandle<SceneInstance>> loadedSceneHandles = new();
        private readonly HashSet<GameObject> instantiatedObjects = new();

        public AddressableAssetService(AddressableInitializer initializer)
        {
            this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }

        public Task InitializeAsync()
        {
           return initializer.InitializeAsync();
        }
        

        public async Task LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            await InitializeAsync();
            if(string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("[AddressableService] LoadSceneAsync key 为空");
                return;
            }
            if(loadedSceneHandles.ContainsKey(key))
            {
                Debug.LogWarning($"[AddressableService] 场景已经加载: {key}");
                return;
            }
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key,loadMode,true);
            await handle.Task;
            if(handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                throw new System.Exception($"[AddressableService] 场景加载失败：{key}");
            }
            if(loadMode == LoadSceneMode.Single)
            {
                loadedSceneHandles.Clear();
            }
            loadedSceneHandles[key] = handle;
            Debug.Log($"[AddressableService] 场景加载完成：{key}");
        }
        public async Task UnloadSceneAsync(string key)
        {
            if(string.IsNullOrEmpty(key))
            {
                Debug.LogError("[AddressableService] UnloadSceneAsync key 为空");
                return;
            }
            if(!loadedSceneHandles.TryGetValue(key, out AsyncOperationHandle<SceneInstance> handle))
            {
                Debug.LogWarning($"[AddressableService] 场景未记录，无法卸载: {key}");
                return;
            }
            var unloadHandle = Addressables.UnloadSceneAsync(handle, true);
            await unloadHandle.Task;
            if (unloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[AddressableService] 场景卸载失败: {key}");
                return;
            }
            loadedSceneHandles.Remove(key);
            Debug.Log($"[AddressableService] 场景卸载完成: {key}");
        }
        public async Task<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            await InitializeAsync();

            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("[AddressableService] LoadAssetAsync key 为空");
                return null;
            }

            //如果加载过
            if (loadedAssetHandles.TryGetValue(key, out AsyncOperationHandle cachedHandle))
            {
                if (cachedHandle.IsValid() && cachedHandle.Result is T cachedAsset)
                {
                    return cachedAsset;
                }

                Debug.LogWarning($"[AddressableService] 缓存资源类型不匹配或已失效: {key}");
                if (cachedHandle.IsValid())
                {
                    Addressables.Release(cachedHandle);
                }
                loadedAssetHandles.Remove(key);
            }
           
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                Debug.LogError($"加载失败：{key}");
                return null;
            }

            loadedAssetHandles[key] = handle;
            return handle.Result;
        }

        public async Task<GameObject> InstantiateAsync(string key, Transform parent = null)
        {
            await InitializeAsync();

            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogError("[AddressableService] InstantiateAsync key 为空");
                return null;
            }

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, parent);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                Debug.LogError($"[AddressableService] 实例化失败: {key}");
                return null;
            }

            GameObject instance = handle.Result;
            instantiatedObjects.Add(instance);

            return instance;
        }

        public void ReleaseAsset(string key)
        {
            if (loadedAssetHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                loadedAssetHandles.Remove(key);
            }
        }
        public void ReleaseInstance(GameObject obj)
        {
            if (obj == null) return;
            bool released = Addressables.ReleaseInstance(obj);
            if(!released)
            {
                Debug.LogWarning($"[AddressableService] ReleaseInstance 失败，可能不是 Addressables.InstantiateAsync 创建的对象: {obj.name}");
            }
            instantiatedObjects.Remove(obj);
        }
        public void ReleaseAll()
        {
            foreach (var obj in instantiatedObjects)
            {
                if (obj != null)
                {
                    Addressables.ReleaseInstance(obj);
                }
            }
            instantiatedObjects.Clear();
            foreach (var handle in loadedAssetHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            loadedAssetHandles.Clear();
        }

    }
}