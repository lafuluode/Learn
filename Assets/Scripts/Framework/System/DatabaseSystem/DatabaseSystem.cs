using System;
using System.IO;
using Game.Framework.Database;
using UnityEngine;

namespace Game.Framework.Core
{
    /// <summary>
    /// 管理本地数据库服务的生命周期。
    /// </summary>
    /// <remarks>
    /// DatabaseSystem 只负责创建、注册和释放 <see cref="IDatabaseService"/>。
    /// 业务模块应通过 ServiceLocator 获取接口，而不应直接持有 SQLiteConnection。
    /// </remarks>
    public sealed class DatabaseSystem : IGameSystem
    {
        public const string DefaultDatabaseFileName = "game.db";

        private readonly string databaseFileName;
        private IDatabaseService databaseService;

        /// <summary>
        /// 数据库不依赖其他游戏系统，需在存档等持久化消费者之前完成注册。
        /// </summary>
        public int Priority => 200;

        public DatabaseSystem(string databaseFileName = DefaultDatabaseFileName)
        {
            if (string.IsNullOrWhiteSpace(databaseFileName))
            {
                throw new ArgumentException("数据库文件名不能为空。", nameof(databaseFileName));
            }

            if (Path.IsPathRooted(databaseFileName) || databaseFileName.Contains(".."))
            {
                throw new ArgumentException("数据库文件名必须是 persistentDataPath 下的相对路径。", nameof(databaseFileName));
            }

            this.databaseFileName = databaseFileName;
        }

        public void OnInit()
        {
            string databasePath = Path.Combine(Application.persistentDataPath, databaseFileName);
            databaseService = new SQLiteDatabaseService(databasePath);

            ServiceLocator.Register<IDatabaseService>(databaseService);
            Debug.Log($"[DatabaseSystem] 数据库已打开: {databasePath}");
        }

        public void OnShutdown()
        {
            ServiceLocator.Unregister<IDatabaseService>();
            databaseService?.Dispose();
            databaseService = null;

            Debug.Log("[DatabaseSystem] 数据库已关闭。");
        }
    }
}
