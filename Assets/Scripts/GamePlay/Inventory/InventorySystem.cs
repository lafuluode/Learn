using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game.Framework.Config;
using Game.Framework.Core;
using UnityEngine;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 背包模块对现有 IGameSystem 生命周期的适配入口。
    /// 当前规模下它直接实现 IInventoryService，不再额外创建 InventoryService。
    /// </summary>
    public sealed class InventorySystem : IGameSystem, IInventoryService
    {
        public const string DefaultItemTableKey = "config/inventory/items";
        private const int DemoInitialItemCount = 60;

        private readonly string itemTableKey;
        private readonly int capacity;
        private CancellationTokenSource initializationCts;
        private Inventory inventory;

        public int Priority => 500;
        public Task Initialization { get; private set; } = Task.CompletedTask;
        public bool IsReady => inventory != null && Initialization.Status == TaskStatus.RanToCompletion;
        public int Capacity => GetInventory().Capacity;
        public int OccupiedSlots => GetInventory().OccupiedSlots;
        public int RemainingSlots => GetInventory().RemainingSlots;

        public event Action Changed;

        public InventorySystem(
            string itemTableKey = DefaultItemTableKey,
            int capacity = Inventory.DefaultCapacity)
        {
            if (string.IsNullOrWhiteSpace(itemTableKey))
            {
                throw new ArgumentException("物品配置表 Key 不能为空。", nameof(itemTableKey));
            }

            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            this.itemTableKey = itemTableKey;
            this.capacity = capacity;
        }

        public void OnInit()
        {
            initializationCts = new CancellationTokenSource();
            Initialization = InitializeAsync(initializationCts.Token);
            ServiceLocator.Register<IInventoryService>(this);
        }

        public void OnShutdown()
        {
            ServiceLocator.Unregister<IInventoryService>();
            initializationCts?.Cancel();
            initializationCts?.Dispose();
            initializationCts = null;

            if (inventory != null)
            {
                inventory.Changed -= OnInventoryChanged;
                inventory = null;
            }

            Initialization = Task.CompletedTask;
        }

        public IReadOnlyList<InventoryItem> GetItems(ItemCategory category = ItemCategory.All)
        {
            return GetInventory().GetItems(category);
        }

        public bool TryGetConfig(int configId, out ItemConfig config)
        {
            return GetInventory().TryGetConfig(configId, out config);
        }

        public bool TryAddItem(int configId, int count = 1)
        {
            return GetInventory().TryAddItem(configId, count);
        }

        public bool TryRemoveItem(int configId, int count = 1)
        {
            return GetInventory().TryRemoveItem(configId, count);
        }

        private Inventory GetInventory()
        {
            if (inventory == null)
            {
                throw new InvalidOperationException("[InventorySystem] 系统尚未初始化。");
            }

            return inventory;
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!ServiceLocator.TryGet(out IConfigService configService))
                {
                    throw new InvalidOperationException("[InventorySystem] IConfigService 尚未注册。");
                }

                if (!configService.HasTable<ItemConfig, int>())
                {
                    await configService.LoadTableAsync<ItemConfigTableData, ItemConfig, int>(
                        itemTableKey,
                        cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();
                IReadOnlyCollection<ItemConfig> configs = configService.GetAll<ItemConfig, int>();
                List<ItemConfig> configList = new(configs);

                // 演示数据填充 60 个独立槽位，以便直接验证三页分页效果。
                // 接入存档后，把这里替换成反序列化得到的 InventoryItem 集合即可。
                List<InventoryItem> initialItems = new();
                if (configList.Count > 0)
                {
                    int demoCount = Math.Min(DemoInitialItemCount, capacity);
                    for (int i = 0; i < demoCount; i++)
                    {
                        initialItems.Add(new InventoryItem(configList[i % configList.Count].Id));
                    }
                }

                inventory = new Inventory(configList, initialItems, capacity);
                inventory.Changed += OnInventoryChanged;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                throw;
            }
        }

        private void OnInventoryChanged()
        {
            Changed?.Invoke();
        }
    }
}
