using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Framework.Config
{
    public class ConfigTable<TConfig, TKey> : IConfigTable<TConfig, TKey>
        where TConfig : class, IConfigRow<TKey>
    {
        /// <summary>
        /// 配置行字典，键为配置行的ID，值为配置行对象
        /// </summary>
        private readonly Dictionary<TKey, TConfig> configs;
        
        public int Count => configs.Count;
        /// <summary>
        /// 构造配置表
        /// </summary>
        /// <param name="rows"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public ConfigTable(IEnumerable<TConfig> rows)
        {
            if(rows == null) throw new ArgumentNullException(nameof(rows));

            configs = new Dictionary<TKey, TConfig>();

            foreach(var row in rows)
            {
                if(row == null) continue;
                TKey id = row.Id;
                if(configs.ContainsKey(id))
                {
                   throw new Exception($"[ConfigTable] 配置表 {typeof(TConfig).Name} 中存在重复的配置行ID: {id}");
                }
                
                configs.Add(id, row);
            }
            
        }
        
        public bool Contains(TKey id)
        {
            return configs.ContainsKey(id);
        }
        
        public TConfig Get(TKey id)
        {
            if(!configs.TryGetValue(id,out TConfig config))
            {
                throw new KeyNotFoundException($"[ConfigTable] 配置表 {typeof(TConfig).Name} 中不存在ID为 {id} 的配置行");
            }
            return config;
        }
        public bool TryGet(TKey id, out TConfig config)
        {
            return configs.TryGetValue(id, out config);
        }
        public IReadOnlyCollection<TConfig> GetAll()
        {
            return configs.Values;
        }
    }
}