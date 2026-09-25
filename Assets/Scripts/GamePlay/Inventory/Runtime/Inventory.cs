using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 纯 C# 背包模型。当前采用非堆叠规则：一件物品占用一个槽位。
    /// </summary>
    public sealed class Inventory
    {
        public const int DefaultCapacity = 80;

        private readonly Dictionary<int, ItemConfig> configs = new();
        private readonly List<InventoryItem> items = new();
        private readonly Dictionary<ItemCategory, IReadOnlyList<InventoryItem>> filteredCache = new();
        private readonly HashSet<long> instanceIds = new();

        private long nextInstanceId = 1;

        public int Capacity { get; }
        public int OccupiedSlots => items.Count;
        public int RemainingSlots => Capacity - OccupiedSlots;
        public bool IsFull => OccupiedSlots >= Capacity;

        public event Action Changed;

        public Inventory(
            IEnumerable<ItemConfig> itemConfigs,
            IEnumerable<InventoryItem> initialItems = null,
            int capacity = DefaultCapacity)
        {
            if (itemConfigs == null)
            {
                throw new ArgumentNullException(nameof(itemConfigs));
            }

            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            Capacity = capacity;

            foreach (ItemConfig config in itemConfigs)
            {
                if (config == null)
                {
                    continue;
                }

                if (config.Id <= 0)
                {
                    throw new ArgumentException("物品配置 Id 必须大于 0。", nameof(itemConfigs));
                }

                if (config.Category == ItemCategory.All)
                {
                    throw new ArgumentException($"物品 {config.Id} 不能使用 All 作为实际分类。", nameof(itemConfigs));
                }

                if (!configs.TryAdd(config.Id, config))
                {
                    throw new ArgumentException($"存在重复的物品配置 Id：{config.Id}", nameof(itemConfigs));
                }
            }

            if (initialItems == null)
            {
                return;
            }

            List<InventoryItem> validInitialItems = new();
            foreach (InventoryItem item in initialItems)
            {
                if (item != null && configs.ContainsKey(item.ConfigId))
                {
                    validInitialItems.Add(item);
                }
            }

            if (validInitialItems.Count > Capacity)
            {
                throw new ArgumentException(
                    $"初始物品数量 {validInitialItems.Count} 超过背包容量 {Capacity}。",
                    nameof(initialItems));
            }

            foreach (InventoryItem item in validInitialItems)
            {
                AddInitialItem(item);
            }
        }

        /// <summary>
        /// 返回分类后的只读列表。相同分类会复用缓存，背包变化时统一失效。
        /// </summary>
        public IReadOnlyList<InventoryItem> GetItems(ItemCategory category = ItemCategory.All)
        {
            if (filteredCache.TryGetValue(category, out IReadOnlyList<InventoryItem> cached))
            {
                return cached;
            }

            List<InventoryItem> result = new();
            foreach (InventoryItem item in items)
            {
                if (category == ItemCategory.All || configs[item.ConfigId].Category == category)
                {
                    result.Add(item);
                }
            }

            ReadOnlyCollection<InventoryItem> readOnly = result.AsReadOnly();
            filteredCache[category] = readOnly;
            return readOnly;
        }

        public bool TryGetConfig(int configId, out ItemConfig config)
        {
            return configs.TryGetValue(configId, out config);
        }

        /// <summary>
        /// 添加 count 件互相独立的物品。容量不足时整次操作失败，不做部分添加。
        /// </summary>
        public bool TryAddItem(int configId, int count = 1)
        {
            if (count <= 0 || !configs.ContainsKey(configId) || count > RemainingSlots)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                items.Add(CreateItem(configId));
            }

            NotifyChanged();
            return true;
        }

        /// <summary>
        /// 按配置 Id 移除 count 件独立物品。数量不足时整次操作失败。
        /// </summary>
        public bool TryRemoveItem(int configId, int count = 1)
        {
            if (count <= 0)
            {
                return false;
            }

            int available = 0;
            foreach (InventoryItem item in items)
            {
                if (item.ConfigId == configId)
                {
                    available++;
                }
            }

            if (available < count)
            {
                return false;
            }

            for (int i = items.Count - 1; i >= 0 && count > 0; i--)
            {
                if (items[i].ConfigId != configId)
                {
                    continue;
                }

                instanceIds.Remove(items[i].InstanceId);
                items.RemoveAt(i);
                count--;
            }

            NotifyChanged();
            return true;
        }

        private void AddInitialItem(InventoryItem item)
        {
            long instanceId = item.InstanceId;
            if (instanceId == 0)
            {
                items.Add(CreateItem(item.ConfigId));
                return;
            }

            if (!instanceIds.Add(instanceId))
            {
                throw new ArgumentException($"存在重复的物品实例 Id：{instanceId}");
            }

            items.Add(new InventoryItem(instanceId, item.ConfigId));
            if (instanceId >= nextInstanceId)
            {
                nextInstanceId = checked(instanceId + 1);
            }
        }

        private InventoryItem CreateItem(int configId)
        {
            while (instanceIds.Contains(nextInstanceId))
            {
                nextInstanceId = checked(nextInstanceId + 1);
            }

            long instanceId = nextInstanceId;
            nextInstanceId = checked(nextInstanceId + 1);
            instanceIds.Add(instanceId);
            return new InventoryItem(instanceId, configId);
        }

        private void NotifyChanged()
        {
            filteredCache.Clear();
            Changed?.Invoke();
        }
    }
}
