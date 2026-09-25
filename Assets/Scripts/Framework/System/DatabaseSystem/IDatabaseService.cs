using System;
using System.Collections.Generic;

namespace Game.Framework.Database
{
    /// <summary>
    /// 本地关系型数据库的业务访问入口。
    /// </summary>
    /// <remarks>
    /// 调用方通过实体类型和参数化 SQL 完成持久化，不直接依赖 SQLiteConnection。
    /// 数据表实体可使用 sqlite-net 的映射特性（例如 PrimaryKey、AutoIncrement）声明主键和列。
    /// </remarks>
    public interface IDatabaseService : IDisposable
    {
        /// <summary>
        /// 当前服务是否仍持有可用连接。
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 当前数据库文件的绝对路径，仅用于诊断和日志。
        /// </summary>
        string DatabasePath { get; }

        /// <summary>
        /// 按实体映射创建或迁移数据表。
        /// </summary>
        void CreateTable<T>() where T : new();

        /// <summary>
        /// 插入一条实体记录。
        /// </summary>
        int Insert<T>(T entity) where T : class, new();

        /// <summary>
        /// 插入或按主键替换一条实体记录。
        /// </summary>
        int InsertOrReplace<T>(T entity) where T : class, new();

        /// <summary>
        /// 更新一条已存在的实体记录。
        /// </summary>
        int Update<T>(T entity) where T : class, new();

        /// <summary>
        /// 删除一条实体记录。
        /// </summary>
        int Delete<T>(T entity) where T : class, new();

        /// <summary>
        /// 按主键删除一条实体记录。
        /// </summary>
        int DeleteByPrimaryKey<T>(object primaryKey) where T : new();

        /// <summary>
        /// 按主键读取一条实体记录；不存在时返回 false。
        /// </summary>
        bool TryGet<T>(object primaryKey, out T entity) where T : class, new();

        /// <summary>
        /// 读取一张表中的全部实体记录。
        /// </summary>
        IReadOnlyList<T> GetAll<T>() where T : new();

        /// <summary>
        /// 执行参数化查询。SQL 中的占位符应使用 ?，避免拼接外部输入。
        /// </summary>
        IReadOnlyList<T> Query<T>(string sql, params object[] args) where T : new();

        /// <summary>
        /// 执行参数化的非查询 SQL，并返回受影响的行数。
        /// </summary>
        int Execute(string sql, params object[] args);

        /// <summary>
        /// 在同一事务内执行一组数据库操作；操作抛出异常时事务会回滚。
        /// </summary>
        void RunInTransaction(Action action);
    }
}
