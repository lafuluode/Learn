using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Game.Framework.Config
{

    public class ConfigService : IConfigService
    {  
        public bool IsInitialized => throw new System.NotImplementedException();

        public IReadOnlyDictionary<int, T> GetAll<T>() where T : class
        {
            throw new System.NotImplementedException();
        }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseAll()
        {
            throw new System.NotImplementedException();
        }

        TConfig IConfigService.Get<TConfig, TKey>(TKey id)
        {
            throw new System.NotImplementedException();
        }

        IConfigTable<TConfig, TKey> IConfigService.GetTable<TConfig, TKey>()
        {
            throw new System.NotImplementedException();
        }

        bool IConfigService.HasTable<TConfig, TKey>()
        {
            throw new System.NotImplementedException();
        }

        Task IConfigService.LoadTableAsync<TConfig, TKey>(string tableKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        Task IConfigService.ReloadTableAsync<TConfig, TKey>(string tableKey, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        void IConfigService.UnloadTable<TConfig, TKey>()
        {
            throw new System.NotImplementedException();
        }
    }
}
