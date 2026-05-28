using MCPForUnity.Editor.Clients.Configurators;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Framework.Config
{
    /// <summary>
    /// 配置表服务接口
    /// 负责配置表的加载、查询、重载和释放
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// 配置服务是否已经初始化完成
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// 初始化配置服务，加载所有配置表数据
        /// 通常在游戏启动时调用，确保配置数据可用
        /// </summary>
        /// <param name="cancellationToken">用于取消初始化操作的令牌</param>
        /// <returns>表示异步操作的任务</returns>
        Task InitializeAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 加载指定类型的配置表
        /// </summary>
        /// <typeparam name="TConfig">
        /// 配置行类型
        /// </typeparam>
        /// <typeparam name="TKey">
        /// 配置主键类型
        /// </typeparam>
        /// <param name="tableKey">
        /// 配置表资源标识，
        /// 该标识只表示"要加载哪张表"，不暴露底层资源系统细讲
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LoadTableAsync<TConfig, TKey>(
            string tableKey,
            CancellationToken cancellationToken = default) 
            where TConfig:class,IConfigRow<TKey> ;

        /// <summary>
        /// 判定指定类型的配置表是否已经加载
        /// </summary>
        /// <typeparam name="TConfig">配置行类型</typeparam>
        /// <typeparam name="TKey">配置主键类型</typeparam>
        /// <returns></returns>
        bool HasTable<TConfig, TKey>() where TConfig : class, IConfigRow<TKey> ;

        /// <summary>
        /// 获取指定类型的配置表，
        /// 如果配置表尚未加载，则抛出异常，或者输出明确错误日志
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        IConfigTable<TConfig, TKey> GetTable<TConfig, TKey>() where TConfig : class, IConfigRow<TKey>;

        /// <summary>
        /// 根据ID获取指定类型的一条配置
        /// </summary>
        /// <typeparam name="TConfig">配置行类型</typeparam>
        /// <typeparam name="TKey">配置主键类型</typeparam>
        /// <param name="id">配置 ID</param>
        /// <returns>查询到的配置行</returns>
        TConfig Get<TConfig,TKey>(TKey id) where TConfig : class, IConfigRow<TKey>;

        /// <summary>
        /// 获取指定类型的所有配置行
        /// </summary>
        /// <typeparam name="T">配置行类型</typeparam>
        /// <returns>包含所有配置行的只读字典，键为配置行的ID</returns>
        IReadOnlyCollection<TConfig> GetAll<TConfig,TKey>() where TConfig : class,IConfigRow<TKey>;
        /// <summary>
        /// 重新加载指定类型的配置表，
        /// 常用于开发调试、GM 工具或热更新后的配置刷新
        /// </summary>
        /// <typeparam name="TConfig">配置行类型</typeparam>
        /// <typeparam name="TKey">配置主键类型</typeparam>
        /// <param name="tableKey">配置表资源标识</param>
        /// <param name="cancellationToken">用于取消操作的令牌</param>
        /// <returns></returns>
        Task ReloadTableAsync<TConfig,TKey>(
            string tableKey,
            CancellationToken cancellationToken = default)
            where TConfig: class,IConfigRow<TKey>;
        /// <summary>
        /// 卸载指定类型的配置表，
        /// 一般只对分模块配置、活动配置、玩法配置使用
        /// 常驻基础配置通常不需要频繁卸载
        /// </summary>
        /// <typeparam name="TConfig">配置行类型，</typeparam>
        /// <typeparam name="TKey">配置主键类型</typeparam>
        void UnloadTable<TConfig, TKey>() where TConfig : class, IConfigRow<TKey>;

        /// <summary>
        /// 释放所有已经加载的配置表
        /// 通常在游戏退出、重新登录、切换大区或清理框架时调用。
        /// </summary>
        void ReleaseAll();

    }
    /// <summary>
    /// 配置表数据行接口
    /// </summary>
    /// <typeparam name="TKey">配置表的主键类型</typeparam>
    public interface IConfigRow<TKey>
    {
        /// <summary>
        /// 配置行唯一表示
        /// </summary>
        TKey Id { get; }
    }

    public interface IConfigTableSource<TConfig,TKey>
        where TConfig :class,IConfigRow<TKey>
    {
        IEnumerable<TConfig> GetRow();
    }


    /// <summary>
    /// 单张配置表接口
    /// 用于管理某一配置类型的所有配置行
    /// </summary>
    /// <typeparam name="TConfig">
    /// 配置行类型，必须实现 IConfigRow<TKey> 接口以确保每行数据具有唯一标识
    /// </typeparam>
    /// <typeparam name="TKey">
    /// 配置主键类型
    /// </typeparam>
    public interface IConfigTable<TConfig,TKey> where TConfig: class,IConfigRow<TKey>
    {
        /// <summary>
        /// 当前配置表中的配置行数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 判定指定 ID 的配置行是否存在于当前配置表中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Contains(TKey id);
        /// <summary>
        /// 根据ID获取配置行数据
        /// </summary>
        /// <param name="id">配置行的唯一标识</param>
        /// <returns>配置行数据，如果不存在则返回 null</returns>
        TConfig Get(TKey id);
        /// <summary>
        /// 尝试根据ID获取配置行数据，如果配置行存在则返回 true，并通过 out 参数返回配置行数据；如果配置行不存在则返回 false，out 参数为 null
        /// </summary>
        /// <param name="id">配置行的唯一标识</param>
        /// <param name="config">输出参数，如果配置行存在则返回配置行数据，否则为 null</param>
        /// <returns>如果配置行存在则返回 true，否则返回 false</returns>
        bool TryGet(TKey id, out TConfig config);
        /// <summary>
        /// 获取当前配置表中所有配置行
        /// </summary>
        /// <returns>全部配置行的只读集合</returns>
        IReadOnlyCollection<TConfig> GetAll();
    }
}