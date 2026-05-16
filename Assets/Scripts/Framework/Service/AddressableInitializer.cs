using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Game.Framework.Resource
{
    /// <summary>
    /// Addressable 初始化器
    /// 职责:
    /// 1.统一封装Addressables.InitializeAsync
    /// 2.避免 AddressableAssetService 和 AddressableUpdateService 各自重复初始化。
    /// 3.维护 Addressables 初始化状态。
    /// </summary>
    public class AddressableInitializer
    {
        private bool isInitialized = false;
        private Task initializeTask;

        /// <summary>
        /// 初始化 Addressables。
        /// 多次调用只会复用同一个初始化任务。
        /// </summary>
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (isInitialized)
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            initializeTask ??= InitializeInternalAsync();

            try
            {
                await initializeTask;
            }
            catch
            {
                initializeTask = null;
                throw;
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        private async Task InitializeInternalAsync()
        {
            var handle = Addressables.InitializeAsync(false);

            try
            {
                await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception("[AddressableInitializer] Addressables 初始化失败");
                }

                isInitialized = true;

                Debug.Log("[AddressableInitializer] Addressables 初始化完成");
            }
            finally
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }
    }
}
