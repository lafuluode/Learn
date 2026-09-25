using System;
using Game.Framework.Config;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 背包页签和物品配置共用的分类。
    /// All 只用于查询，不应作为某个物品的实际分类。
    /// </summary>
    public enum ItemCategory
    {
        All = 0,
        Equipment = 1,
        Consumable = 2,
        Material = 3,
        Fragment = 4,
        Other = 5,
    }

    /// <summary>
    /// 物品静态配置。运行时背包只保存 ConfigId 和数量。
    /// </summary>
    [Serializable]
    public sealed class ItemConfig : IConfigRow<int>
    {
        public int ConfigId;
        public string Name;
        public string Description;
        public string IconKey;
        public ItemCategory Category;

        public int Id => ConfigId;
    }
}
