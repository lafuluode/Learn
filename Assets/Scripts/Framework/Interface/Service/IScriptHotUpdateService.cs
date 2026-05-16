using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Framework.HotUpdate
{
    /// <summary>
    /// 脚本热更新服务接口
    /// 
    /// 负责初始化脚本运行环境，并启动热更层逻辑入口
    /// 具体实现可以是 xLua、HybridCLR、ILRuntime 等第三方热更框架，也可以是自定义的热更方案
    /// </summary>
    public interface IScriptHotUpdateService
    {
        /// <summary>
        /// 初始化脚本热更新环境
        /// 
        /// 例如：
        /// xlua 方案中创建 LuaEnv 实例;
        /// HybridCLR 方案中加载热更程序集并准备运行环境;
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 启动热更层逻辑入口
        /// 
        /// 例如：
        /// Xlua 方案中执行 Main.lua;
        /// HybridCLR 方案中调用热更程序集中的入口方法;
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 关闭脚本热更新环境，并释放相关资源
        /// 
        /// 例如：
        /// Xlua 方案中调用 LuaEnv.Dispose() 释放 Lua 环境;
        /// HybridCLR 方案中卸载热更程序集并清理相关资源;
        /// </summary>
        void Shutdown();
    }

}