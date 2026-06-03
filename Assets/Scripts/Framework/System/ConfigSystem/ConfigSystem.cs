using Game.Framework.Config;
using Game.Framework.Resource;
using Game.Framework.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace Game.Framework.Core
{
    /// <summary>
    /// 配置表系统
    /// </summary>
    /// <remarks>
    /// ConfigSystem 负责管理配置表服务的生命周期。
    /// 
    /// 它只负责创建、初始化、注册和释放 IConfigService。
    /// 具体配置表的加载、缓存和查询由 ConfigService 负责。
    /// </remarks>

    public class ConfigSystem : IGameSystem
    {
        private IConfigService configService;
        private CancellationTokenSource cancellationTokenSource;
        /// <summary>
        /// ConfigSystem 依赖于 ResourceSystem
        /// </summary>
        public int Priority => 400;

        public void OnInit()
        {
            if(!ServiceLocator.TryGet<IAssetService>(out IAssetService assetService))
            {
                throw new InvalidOperationException("[ConfigSystem] 初始化失败：IAssetService 尚未注册。");
            }
            cancellationTokenSource = new CancellationTokenSource();

            IDataSerializer serializer = new UnityJsonDataSerializer();

            configService = new ConfigService(assetService, serializer);

            configService
                .InitializeAsync(cancellationTokenSource.Token)
                .GetAwaiter()
                .GetResult();
            
            ServiceLocator.Register<IConfigService>(configService);
            Debug.Log("[ConfigSystem] 配置表系统初始化完成");
        }

        public void OnShutdown()
        {
            cancellationTokenSource?.Cancel();
            if (configService != null)
            {
                configService.ReleaseAll();
            }
            ServiceLocator.Unregister<IConfigService>();

            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            configService = null;

            Debug.Log("[ConfigSystem] 配置表系统已关闭");
        }
    }
}