
using Game.Framework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Framework.HotUpdate
{
    /// <summary>
    /// 基于HybridCLR实现的脚本热更新服务
    /// 
    /// 负责加载 C# 热更程序集，并调用热更层入口方法。
    /// </summary>
    public class HybridClrScriptHotUpdateService : IScriptHotUpdateService
    {
        private readonly ScriptHotUpdateConfig config;
        private readonly IAssetService assetService;

        private Assembly hotUpdateAssembly;
        private bool isInitialized;
        private bool isStarted;

        public HybridClrScriptHotUpdateService(ScriptHotUpdateConfig config, IAssetService assetService)
        {
            this.config = config?? throw new System.ArgumentNullException(nameof(config));
            this.assetService = assetService ?? throw new System.ArgumentNullException(nameof(assetService));
        }


        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(isInitialized)
            {
                return Task.CompletedTask;
            }
            Debug.Log("[HybridClrScriptHotUpdateService] 初始化开始");

            // 第一版先不处理 AOT 元数据补充。
            // 后续可以在这里调用 HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly。

            isInitialized = true;

            Debug.Log("[HybridClrScriptHotUpdateService] 初始化完成");

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(isInitialized)
            {
                await InitializeAsync(cancellationToken);
            }
            if(isStarted)
            {
                Debug.LogWarning("[HybridClrScriptHotUpdateService] 热更入口已经启动，忽略重复启动");
                return;
            }
            Debug.Log("[HybridClrScriptHotUpdateService] 开始加载热更程序集");
            TextAsset dllAsset = await assetService.LoadAssetAsync<TextAsset>(config.hybridClrAssemblyKey);

            cancellationToken.ThrowIfCancellationRequested();
            if(dllAsset == null)
            {
                throw new Exception(
                     $"[HybridClrScriptHotUpdateService] 加载热更程序集失败，资源 Key: {config.hybridClrAssemblyKey}");
            }

            byte[] dllBytes = dllAsset.bytes;
            if (dllBytes == null || dllBytes.Length == 0)
            {
                throw new Exception(
                    $"[HybridClrScriptHotUpdateService] 加载热更程序集失败，资源 Key: {config.hybridClrAssemblyKey}");
            }

            hotUpdateAssembly = Assembly.Load(dllBytes);
            Debug.Log($"[HybridClrScriptHotUpdateService] 热更程序集加载成功: {hotUpdateAssembly.FullName}");
            InvokeEntryMethod();
            isStarted = true;
            Debug.Log("[HybridClrScriptHotUpdateService] 热更入口启动完成");
        }
        /// <summary>
        /// 调用热更程序集入口方法
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InvokeEntryMethod()
        {
            if(hotUpdateAssembly == null)
            {
                throw new Exception("[HybridClrScriptHotUpdateService] 热更程序集未加载");
            }
            Type entryType = hotUpdateAssembly.GetType(config.hybridClrEntryTypeName);

            if(entryType == null )
            {
                throw new Exception(
                    $"[HybridClrScriptHotUpdateService] 找不到热更入口类型: {config.hybridClrEntryTypeName}");
            }
            MethodInfo entryMethod = entryType.GetMethod(config.hybridClrEntryTypeName,BindingFlags.Public|BindingFlags.Static);

            if( entryMethod == null )
            {
                throw new Exception(
                    $"[HybridClrScriptHotUpdateService] 找不到热更入口方法：{config.hybridClrEntryTypeName}.{config.hybridClrEntryMethodName}");
            }
            entryMethod.Invoke( null, null );
        }
        /// <summary>
        /// 关闭Hybrid脚本热更新服务
        /// 
        /// 注意：
        /// .NET Framework 不支持卸载单个程序集，只有卸载整个 AppDomain 才能释放程序集资源。
        /// 所有这里主要是清理当前服务持有的引用和状态
        /// </summary>
        public void Shutdown()
        {
            hotUpdateAssembly = null;
            isStarted = false;
            isInitialized = false;

            Debug.Log("[HybridClrScriptHotUpdateService] 热更服务已关闭，相关资源已释放");
        }
    }
}