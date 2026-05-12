using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Framework.Core
{
    /// <summary>
    ///资源更新服务接口
    /// 负责资源系统初始化、Catalog 检查、Catalog 更新、资源下载和缓存处理。
    /// 不负责资源实例化和场景加载
    /// </summary>
    public interface IResourceUpdateService
    {
        /// <summary>
        /// 初始化资源更新服务
        /// 例如初始化资源系统、加载远程 Catalog等
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查是否存在资源目录更新
        /// 返回需要更新的 Catalog 列表
        /// </summary>
        /// <returns></returns>
        Task<List<string>> CheckCatalogUpdatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新资源目录
        /// 目录更新后，客户端才能知道有哪些资源包需要下载
        /// </summary>
        /// <param name="catalogs"></param>
        /// <returns></returns>
        Task UpdateCatalogsAsync(List<string> catalogs,CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取某个资源标签或资源 Key 对应的下载大小
        /// 常用于下载前显示“需要下载多少MB”
        /// </summary>
        /// <param name="keyOrLabel"></param>
        /// <returns></returns>
        Task<long> GetDownloadSizeAsync(string keyOrLabel, CancellationToken cancellationToken = default);

        /// <summary>
        /// 下载某个资源标签或资源 Key 对应的依赖资源
        /// 常用于下载 Preload、UI、登陆场景、基础配置表等必要资源
        /// </summary>
        /// <param name="keyOrLabel"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DownloadDependenciesAsync(
            string keyOrLabel,
            IProgress<ResourceDownloadProgress> progress = null,
            CancellationToken cancellationToken = default
            );
        /// <summary>
        /// 清理资源系统缓存
        /// 一般用于调试、版本切换、资源异常修复等情况
        /// </summary>
        /// <returns></returns>
        Task ClearCacheAsync();
    }
}