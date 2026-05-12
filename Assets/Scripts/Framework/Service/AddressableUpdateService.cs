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
       

        public Task UpdateCatalogsAsync(List<string> catalogs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<List<string>> CheckCatalogUpdatesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task DownloadDependenciesAsync(string keyOrLabel, IProgress<ResourceDownloadProgress> progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetDownloadSizeAsync(string keyOrLabel, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task ClearCacheAsync()
        {
            throw new NotImplementedException();
        }

    }
}