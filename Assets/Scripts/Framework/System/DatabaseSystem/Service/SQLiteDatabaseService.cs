using System;
using System.Collections.Generic;
using System.IO;
using SQLite;

namespace Game.Framework.Database
{
    /// <summary>
    /// 基于 sqlite-net 的本地数据库服务实现。
    /// </summary>
    /// <remarks>
    /// sqlite-net 的同步连接由该服务串行访问；查询结果会在锁内物化，避免将延迟枚举器或连接对象泄露到业务层。
    /// </remarks>
    public sealed class SQLiteDatabaseService : IDatabaseService
    {
        private readonly SQLiteConnection connection;
        private readonly object syncRoot = new object();
        private bool isDisposed;

        public bool IsOpen
        {
            get
            {
                lock (syncRoot)
                {
                    return !isDisposed;
                }
            }
        }

        public string DatabasePath { get; }

        public SQLiteDatabaseService(string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentException("数据库路径不能为空。", nameof(databasePath));
            }

            DatabasePath = Path.GetFullPath(databasePath);
            string directory = Path.GetDirectoryName(DatabasePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            connection = new SQLiteConnection(DatabasePath);
            connection.Execute("PRAGMA foreign_keys = ON;");
        }

        public void CreateTable<T>() where T : new()
        {
            lock (syncRoot)
            {
                EnsureOpen();
                connection.CreateTable<T>();
            }
        }

        public int Insert<T>(T entity) where T : class, new()
        {
            ValidateEntity(entity);

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Insert(entity);
            }
        }

        public int InsertOrReplace<T>(T entity) where T : class, new()
        {
            ValidateEntity(entity);

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.InsertOrReplace(entity);
            }
        }

        public int Update<T>(T entity) where T : class, new()
        {
            ValidateEntity(entity);

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Update(entity);
            }
        }

        public int Delete<T>(T entity) where T : class, new()
        {
            ValidateEntity(entity);

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Delete(entity);
            }
        }

        public int DeleteByPrimaryKey<T>(object primaryKey) where T : new()
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Delete<T>(primaryKey);
            }
        }

        public bool TryGet<T>(object primaryKey, out T entity) where T : class, new()
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            lock (syncRoot)
            {
                EnsureOpen();
                entity = connection.Find<T>(primaryKey);
                return entity != null;
            }
        }

        public IReadOnlyList<T> GetAll<T>() where T : new()
        {
            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Table<T>().ToList();
            }
        }

        public IReadOnlyList<T> Query<T>(string sql, params object[] args) where T : new()
        {
            ValidateSql(sql);

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Query<T>(sql, args);
            }
        }

        public int Execute(string sql, params object[] args)
        {
            ValidateSql(sql);

            lock (syncRoot)
            {
                EnsureOpen();
                return connection.Execute(sql, args);
            }
        }

        public void RunInTransaction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            lock (syncRoot)
            {
                EnsureOpen();
                connection.RunInTransaction(action);
            }
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                if (isDisposed)
                {
                    return;
                }

                connection.Dispose();
                isDisposed = true;
            }
        }

        private void EnsureOpen()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(SQLiteDatabaseService));
            }
        }

        private static void ValidateEntity<T>(T entity) where T : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }

        private static void ValidateSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException("SQL 不能为空。", nameof(sql));
            }
        }
    }
}
