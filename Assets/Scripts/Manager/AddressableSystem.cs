using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Core
{
    /// <summary>
    /// addressable系统,管理资源服务实例的生命周期
    /// 负责在游戏启动时注册资源服务实例到服务定位器中，并在游戏关闭时释放资源和注销服务
    /// </summary>
    public class AddressableSystem : IGameSystem
    {
        //持有一个资源服务实例，负责具体的服务提供注册到服务定位器中
        AddressableService assetsService;
        public int Priority => 100;

        public void OnInit()
        {
            assetsService = new AddressableService();
            ServiceLocator.Register<IAssetService>(assetsService);
        }

        public void OnShutdown()
        {
            ServiceLocator.Unregister<IAssetService>();
            assetsService.ReleaseAll();
        }

    }
}