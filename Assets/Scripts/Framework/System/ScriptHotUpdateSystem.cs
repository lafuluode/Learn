using Game.Framework.Core;
using Game.Framework.HotUpdate;
using Game.Framework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Framework.Core
{
    public class ScriptHotUpdateSystem : IGameSystem
    {
        private readonly ScriptHotUpdateConfig config;
        private IScriptHotUpdateService scriptHotUpdateService;
        private bool isInitialized;
        public int Priority => 200;
        public ScriptHotUpdateSystem(ScriptHotUpdateConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// 初始化脚本热更新系统，准备加载热更程序集。
        /// </summary>
        public void OnInit()
        {
            if (isInitialized)
            {
                Debug.LogWarning("[ScriptHotUpdateSystem] 已经初始化，忽略重复初始化");
                return;
            }
            Debug.Log("[ScriptHotUpdateSystem] 初始化开始");
            scriptHotUpdateService = CreateScriptHotUpdateService();
            ServiceLocator.Register<IScriptHotUpdateService>(scriptHotUpdateService);
            isInitialized = true;
        }
        /// <summary>
        /// 根据配置创建具体脚本热更新服务
        /// HybridCLR与Xlua
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private IScriptHotUpdateService CreateScriptHotUpdateService()
        {
            IAssetService assetService = ServiceLocator.Get<IAssetService>();
            return new HybridClrScriptHotUpdateService(config, assetService);
        }
            

        public void OnShutdown()
        {
            if(!isInitialized)
            {
                return;
            }
            scriptHotUpdateService?.Shutdown();
            scriptHotUpdateService = null;
            ServiceLocator.Unregister<IScriptHotUpdateService>();
            isInitialized = false;
            Debug.Log("[ScriptHotUpdateSystem] 已关闭");
        }
    }
}