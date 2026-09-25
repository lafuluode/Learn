using System;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 玩家拥有的一件物品。当前不做堆叠，因此每个实例独占一个背包槽位。
    /// </summary>
    public sealed class InventoryItem
    {
        /// <summary>
        /// 运行时实例编号。0 表示尚未被某个 Inventory 接管并分配编号。
        /// </summary>
        public long InstanceId { get; }

        public int ConfigId { get; }

        public InventoryItem(int configId)
            : this(0, configId)
        {
        }

        public InventoryItem(long instanceId, int configId)
        {
            if (instanceId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId));
            }

            if (configId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(configId));
            }

            InstanceId = instanceId;
            ConfigId = configId;
        }
    }
}
