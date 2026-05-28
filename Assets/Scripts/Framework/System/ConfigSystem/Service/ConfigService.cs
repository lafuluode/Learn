using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Game.Framework.Config
{

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
