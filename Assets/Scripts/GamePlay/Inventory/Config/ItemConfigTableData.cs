using System;
using System.Collections.Generic;
using Game.Framework.Config;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 适配现有 ConfigService 的 JSON 根对象。
    /// JSON 顶层字段名必须是 Items。
    /// </summary>
    [Serializable]
    public sealed class ItemConfigTableData : IConfigTableSource<ItemConfig, int>
    {
        public List<ItemConfig> Items = new();

        public IEnumerable<ItemConfig> GetRows()
        {
            return Items;
        }
    }
}
