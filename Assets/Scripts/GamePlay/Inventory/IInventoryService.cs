using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// UI 和其他玩法访问背包的最小入口。
    /// </summary>
    public interface IInventoryService
    {
        event Action Changed;

        /// <summary>
        /// 物品配置和运行时数据的异步初始化任务。
        /// 调用方应等待它完成后再读取背包。
        /// </summary>
        Task Initialization { get; }

        bool IsReady { get; }

        int Capacity { get; }
        int OccupiedSlots { get; }
        int RemainingSlots { get; }

        IReadOnlyList<InventoryItem> GetItems(ItemCategory category = ItemCategory.All);
        /// <summary>
        /// 获取配置表数据，若不存在则返回 false。
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        bool TryGetConfig(int configId, out ItemConfig config);
        /// <summary>
        /// 尝试添加 count 件独立物品，配置不存在或容量不足时返回 false。
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool TryAddItem(int configId, int count = 1);
        /// <summary>
        /// 尝试移除 count 件相同配置的独立物品，数量不足时返回 false。
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool TryRemoveItem(int configId, int count = 1);
    }
}
