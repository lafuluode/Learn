using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Framework.Resource;

namespace Game.Framework.Core
{
    /// <summary>
    /// Resource系统,管理资源服务实例的生命周期
    /// 负责在游戏启动时注册资源服务实例到服务定位器中，并在游戏关闭时释放资源和注销服务
    /// </summary>
    public class ResourceSystem : IGameSystem
    {
        AddressableInitializer initializer;
        //持有资源服务实例，负责具体的服务提供注册到服务定位器中
        IAssetService assetsService;
        IResourceUpdateService updateResourceService;
        public int Priority => 100;

        public void OnInit()
        {
            initializer = new AddressableInitializer();
            assetsService = new AddressableAssetService(initializer);
            updateResourceService = new AddressableUpdateService(initializer);
            ServiceLocator.Register<IAssetService>(assetsService);
            ServiceLocator.Register<IResourceUpdateService>(updateResourceService);
        }

        public void OnShutdown()
        {
            ServiceLocator.Unregister<IAssetService>();
            ServiceLocator.Unregister<IResourceUpdateService>();
            assetsService.ReleaseAll();
        }

    }
}