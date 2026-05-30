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
        public bool IsInitialized { get; private set; }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task LoadTableAsync<TConfig, TKey>(
            string tableKey,
            CancellationToken cancellationToken = default)
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        public bool HasTable<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        public IConfigTable<TConfig, TKey> GetTable<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        public TConfig Get<TConfig, TKey>(TKey id)
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        public void UnloadTable<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        

        public IReadOnlyCollection<TConfig> GetAll<TConfig, TKey>()
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        public Task ReloadTableAsync<TConfig, TKey>(string tableKey, CancellationToken cancellationToken)
            where TConfig : class, IConfigRow<TKey>
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseAll()
        {
            throw new System.NotImplementedException();
        }
    }
}
