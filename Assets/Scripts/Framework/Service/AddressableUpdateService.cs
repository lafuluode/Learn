using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Framework.Core
{
    /// <summary>
    /// 基于 Addressables 实现的资源更新服务
    /// 负责初始化、检查 Catalog、更新 Catalog、计算下载大小、下载依赖资源
    /// </summary>
    public class AddressableUpdateService : IResourceUpdateService
    {
        private readonly AddressableInitializer initializer;
        public AddressableUpdateService(AddressableInitializer initializer)
        {
            this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }
        
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            return initializer.InitializeAsync(cancellationToken);
        }
       

        public async Task UpdateCatalogsAsync(List<string> catalogs, CancellationToken cancellationToken = default)
        {
            await initializer.InitializeAsync(cancellationToken);

            if(catalogs == null || catalogs.Count == 0)
            {
                Debug.LogWarning("[AddressableUpdateService] 没有需要更新的 Catalog");
                return;
            }
            cancellationToken.ThrowIfCancellationRequested();
            var handle = Addressables.UpdateCatalogs(catalogs);

            try
            {
                await handle.Task;
                cancellationToken.ThrowIfCancellationRequested();
                if(handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception("[AddressableUpdateService] 更新 Catalog 失败");
                }
                Debug.Log($"[AddressableUpdateService] 更新 Catalog 完成");
            }
            finally
            {                 
                if(handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

        }

        public async Task<List<string>> CheckCatalogUpdatesAsync(CancellationToken cancellationToken = default)
        {
            await initializer.InitializeAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            var handle = Addressables.CheckForCatalogUpdates(false);

            try
            {
                await handle.Task;

                cancellationToken.ThrowIfCancellationRequested();
                if(handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception("[AddressableUpdateService] 检查 Catalog 更新失败");
                }
                return handle.Result;
            }
            finally
            {
                if(handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }
        public async Task<long> GetDownloadSizeAsync(string keyOrLabel, CancellationToken cancellationToken = default)
        {
            await initializer.InitializeAsync(cancellationToken);
            if(string.IsNullOrWhiteSpace(keyOrLabel))
            {
                throw new ArgumentException("[AddressableUpdateService] keyOrLabel 为空", nameof(keyOrLabel));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var handle = Addressables.GetDownloadSizeAsync(keyOrLabel);
            try 
            {
                await handle.Task;
                cancellationToken.ThrowIfCancellationRequested();
                if(handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"[AddressableUpdateService] 获取下载大小失败: {keyOrLabel}");
                }
                return handle.Result;
            }
            finally
            {
                if(handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }

        public async Task DownloadDependenciesAsync(string keyOrLabel, IProgress<ResourceDownloadProgress> progress = null, CancellationToken cancellationToken = default)
        {
            await initializer.InitializeAsync(cancellationToken);

            if(string.IsNullOrWhiteSpace(keyOrLabel))
            {
                throw new ArgumentException("[AddressableUpdateService] keyOrLabel 为空", nameof(keyOrLabel));
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            long downloadedSize = await GetDownloadSizeAsync(keyOrLabel, cancellationToken);

            if(downloadedSize <= 0)
            {
                progress?.Report(new ResourceDownloadProgress
                {
                    DownloadedBytes = 0,
                    TotalBytes = 0,
                    Percent = 1f,
                    Message = "资源已是最新，无需下载"
                });
                Debug.Log($"[AddressableUpdateService] 资源已是最新，无需下载: {keyOrLabel}");
                return;
            }
            
            var handle = Addressables.DownloadDependenciesAsync(keyOrLabel, false);

            try
            {
                while (!handle.IsDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var status = handle.GetDownloadStatus();
                    progress?.Report(new ResourceDownloadProgress(
                        downloadedBytes: status.DownloadedBytes,
                        totalBytes: status.TotalBytes,
                        percent: status.Percent,
                        message: "正在下载资源..."
                        )
                    );
                    await Task.Yield();
                }
                await handle.Task;
                cancellationToken.ThrowIfCancellationRequested();

                if(handle.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"[AddressableUpdateService] 下载依赖失败: {keyOrLabel}");
                }

                var finalStatus = handle.GetDownloadStatus();
                progress?.Report(new ResourceDownloadProgress(
                    downloadedBytes: finalStatus.DownloadedBytes,
                    totalBytes: finalStatus.TotalBytes,
                    percent: 1f,
                    message: "资源下载完成"
                    )
                );
                Debug.Log($"[AddressableUpdateService] 资源下载完成: {keyOrLabel}");
            }
            finally
            {
                if(handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }

        
        public Task ClearCacheAsync()
        {
            Debug.LogWarning("[AddressableUpdateService] ClearCacheAsync 当前暂未完整实现");
            return Task.CompletedTask;
        }

    }
}