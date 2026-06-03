using Game.Framework.Resource;
using Game.Framework.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Game.Framework.Config
{
    /// <summary>
    /// 配置表服务的默认实现
    /// </summary>
    /// <remarks>
    /// 负责加载、缓存、查询和释放配置表
    /// ConfigService 不直接关心配置表底层来自 Addressables、AssetBundle 还是其他资源系统，
    /// 它只通过 IAssetService 加载 TextAsset。
    /// 
    /// ConfigService 也不直接关心具体序列化格式，
    /// 它只通过 IDataSerializer 将字节数据反序列化为配置表数据对象。
    /// </remarks>
    public class ConfigService : IConfigService
    {
        private readonly IAssetService assetService;
        private readonly IDataSerializer dataSerializer;
        /// <summary>
        /// 已加载配置表缓存
        /// key: 配置行类型，例如 typeof(ItemConfig)
        /// value: IConfigTable<TConfig, TKey>，由于泛型不同，所以用 object 保存。
        /// </summary>
        private readonly Dictionary<Type,object> tables = new();

        public bool IsInitialized { get; private set; }

        public ConfigService(IAssetService assetService, IDataSerializer dataSerializer)
        {
            this.assetService = assetService ?? throw new ArgumentNullException(nameof(assetService));
            this.dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
        }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IsInitialized = true;

            return Task.CompletedTask;
        }

        public async Task LoadTableAsync<TTableData,TConfig, TKey>(
            string tableKey,
            CancellationToken cancellationToken = default)
            where TTableData : IConfigTableSource<TConfig, TKey>
            where TConfig : class, IConfigRow<TKey>
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(string.IsNullOrWhiteSpace(tableKey))
            {
                throw new ArgumentException("配置表资源 key 不能为空", nameof(tableKey));
            }
            Type tableType = GetTableKey<TConfig>();

            if(tables.ContainsKey(tableType))
            {
                Debug.LogWarning($"[ConfigService] 配置表已经加载：{typeof(TConfig).Name}");
                return;
            }

            TextAsset textAsset = await assetService.LoadAssetAsync<TextAsset>(tableKey);

            cancellationToken.ThrowIfCancellationRequested();

            if (textAsset == null)
            {
                throw new InvalidOperationException($"配置表资源加载失败：{tableKey}");
            }

            TTableData tableData = dataSerializer.Deserialize<TTableData>(textAsset.bytes);

            if(tableData == null)
            {
                throw new InvalidOperationException($"配置表反序列化失败：{tableKey}");
            }
            IEnumerable<TConfig> rows = tableData.GetRows();

            ConfigTable<TConfig,TKey> table = new ConfigTable<TConfig, TKey>(rows);

            tables.Add(tableType, table);

            Debug.Log($"[ConfigService] 配置表加载完成：{typeof(TConfig).Name}，数量：{table.Count}");
        }

        public bool HasTable<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            Type tableType = GetTableKey<TConfig>();

            return tables.ContainsKey(tableType);
        }

        public IConfigTable<TConfig, TKey> GetTable<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            Type table = GetTableKey<TConfig>();

            if(!tables.TryGetValue(table,out object tableObj))
            {
                throw new InvalidOperationException($"配置表未加载：{typeof(TConfig).Name}");
            }

            return (IConfigTable<TConfig, TKey>)tableObj;
        }

        public TConfig Get<TConfig, TKey>(TKey id)
            where TConfig : class, IConfigRow<TKey>
        {
            IConfigTable<TConfig, TKey> table = GetTable<TConfig, TKey>();

            return table.Get(id);
        }

        public void UnloadTable<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            Type tableType = GetTableKey<TConfig>();
            tables.Remove(tableType);
        }

        public IReadOnlyCollection<TConfig> GetAll<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            return GetTable<TConfig, TKey>().GetAll();
        }

        public async Task ReloadTableAsync<TTableData,TConfig, TKey>(string tableKey, CancellationToken cancellationToken)
            where TTableData : IConfigTableSource<TConfig, TKey>
            where TConfig : class, IConfigRow<TKey>
        {
            UnloadTable<TConfig, TKey>();
            await LoadTableAsync<TTableData, TConfig, TKey>(tableKey, cancellationToken);
        }

        public void ReleaseAll()
        {
            tables.Clear();

            IsInitialized = false;
        }
        private static Type GetTableKey<TConfig>()
        {
            return typeof(TConfig);
        }
    }
}
