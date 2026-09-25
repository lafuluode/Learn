#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Framework.Core;
using Game.Framework.Resource;
using Game.Framework.Serialization;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.GamePlay.Inventory.Tests
{
    public sealed class InventoryTests
    {
        private static ItemConfig Equipment(int id)
        {
            return new ItemConfig
            {
                ConfigId = id,
                Name = $"Equipment-{id}",
                Category = ItemCategory.Equipment,
            };
        }

        private static ItemConfig Material(int id)
        {
            return new ItemConfig
            {
                ConfigId = id,
                Name = $"Material-{id}",
                Category = ItemCategory.Material,
            };
        }

        [Test]
        public void GetItems_FiltersByCategoryAndReusesCache()
        {
            ItemConfig[] configs = { Equipment(1), Material(2), Equipment(3) };
            Inventory inventory = new(configs, new[]
            {
                new InventoryItem(1),
                new InventoryItem(2),
                new InventoryItem(3),
            });

            IReadOnlyList<InventoryItem> first = inventory.GetItems(ItemCategory.Equipment);
            IReadOnlyList<InventoryItem> second = inventory.GetItems(ItemCategory.Equipment);

            Assert.That(first.Count, Is.EqualTo(2));
            Assert.That(first[0].ConfigId, Is.EqualTo(1));
            Assert.That(first[1].ConfigId, Is.EqualTo(3));
            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void TryAddItem_CreatesSeparateSlotsAndInvalidatesFilterCache()
        {
            ItemConfig[] configs = { Equipment(1) };
            Inventory inventory = new(configs, new[]
            {
                new InventoryItem(1),
                new InventoryItem(1),
            });
            IReadOnlyList<InventoryItem> before = inventory.GetItems();
            int changedCount = 0;
            inventory.Changed += () => changedCount++;

            bool added = inventory.TryAddItem(1, 3);
            IReadOnlyList<InventoryItem> after = inventory.GetItems();

            Assert.That(added, Is.True);
            Assert.That(after, Has.Count.EqualTo(5));
            Assert.That(after, Has.All.Matches<InventoryItem>(item => item.ConfigId == 1));
            Assert.That(
                new HashSet<long>(System.Array.ConvertAll(
                    System.Linq.Enumerable.ToArray(after),
                    item => item.InstanceId)),
                Has.Count.EqualTo(5));
            Assert.That(inventory.OccupiedSlots, Is.EqualTo(5));
            Assert.That(inventory.RemainingSlots, Is.EqualTo(75));
            Assert.That(after, Is.Not.SameAs(before));
            Assert.That(changedCount, Is.EqualTo(1));
        }

        [Test]
        public void TryAddItem_WhenCapacityIsInsufficient_FailsAtomically()
        {
            Inventory inventory = new(new[] { Equipment(1) }, capacity: 3);
            int changedCount = 0;
            inventory.Changed += () => changedCount++;

            Assert.That(inventory.TryAddItem(1, 2), Is.True);
            Assert.That(inventory.TryAddItem(1, 2), Is.False);
            Assert.That(inventory.OccupiedSlots, Is.EqualTo(2));
            Assert.That(inventory.RemainingSlots, Is.EqualTo(1));
            Assert.That(changedCount, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveItem_RemovesSeparateInstances()
        {
            Inventory inventory = new(new[] { Equipment(1) });
            Assert.That(inventory.TryAddItem(1, 3), Is.True);

            Assert.That(inventory.TryRemoveItem(1, 2), Is.True);
            Assert.That(inventory.GetItems(), Has.Count.EqualTo(1));
            Assert.That(inventory.OccupiedSlots, Is.EqualTo(1));
        }

        [Test]
        public void Inventory_RejectsInitialItemsBeyondCapacity()
        {
            InventoryItem[] initialItems =
            {
                new InventoryItem(1),
                new InventoryItem(1),
            };

            Assert.Throws<System.ArgumentException>(() =>
                new Inventory(new[] { Equipment(1) }, initialItems, capacity: 1));
        }

        [Test]
        public void InventoryView_UsesSevenByFourPageSize()
        {
            Assert.That(InventoryView.PageSize, Is.EqualTo(7 * 4));
            Assert.That(Inventory.DefaultCapacity, Is.EqualTo(80));
        }

        [Test]
        public void TryAddItem_RejectsUnknownConfig()
        {
            Inventory inventory = new(new[] { Equipment(1) });

            Assert.That(inventory.TryAddItem(999), Is.False);
            Assert.That(inventory.GetItems(), Is.Empty);
        }

        [Test]
        public void ItemConfigJson_UsesTheExpectedConfigServiceShape()
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                "Assets/ConfigTable/Inventory/ItemConfig.json");
            UnityJsonDataSerializer serializer = new();

            Assert.That(textAsset, Is.Not.Null);
            ItemConfigTableData table = serializer.Deserialize<ItemConfigTableData>(textAsset.bytes);

            Assert.That(table, Is.Not.Null);
            Assert.That(table.Items.Count, Is.EqualTo(8));
            Assert.That(table.Items[0].ConfigId, Is.EqualTo(1001));
            Assert.That(table.Items[0].Category, Is.EqualTo(ItemCategory.Equipment));
        }

        [Test]
        public void ItemConfigJson_IsRegisteredWithTheExpectedAddress()
        {
            const string path = "Assets/ConfigTable/Inventory/ItemConfig.json";
            string guid = AssetDatabase.AssetPathToGUID(path);
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.FindAssetEntry(guid);

            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.address, Is.EqualTo(InventorySystem.DefaultItemTableKey));
        }

        [TestCase("Assets/Art/Inventory/Icons/starter_sword.png", "inventory/icon/starter_sword")]
        [TestCase("Assets/Art/Inventory/Icons/guard_helmet.png", "inventory/icon/guard_helmet")]
        [TestCase("Assets/Art/Inventory/Icons/health_potion.png", "inventory/icon/health_potion")]
        [TestCase("Assets/Art/Inventory/Icons/energy_potion.png", "inventory/icon/energy_potion")]
        [TestCase("Assets/Art/Inventory/Icons/iron_ore.png", "inventory/icon/iron_ore")]
        [TestCase("Assets/Art/Inventory/Icons/magic_crystal.png", "inventory/icon/magic_crystal")]
        [TestCase("Assets/Art/Inventory/Icons/map_fragment.png", "inventory/icon/map_fragment")]
        [TestCase("Assets/Art/Inventory/Icons/commemorative_badge.png", "inventory/icon/commemorative_badge")]
        public void InventoryIcon_IsASpriteWithTheExpectedAddress(string path, string address)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            string guid = AssetDatabase.AssetPathToGUID(path);
            var entry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);

            Assert.That(sprite, Is.Not.Null);
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.address, Is.EqualTo(address));
        }

        [UnityTest]
        public IEnumerator InventorySystem_InitializesThroughTheRealAddressablesChain()
        {
            ServiceLocator.Clear();
            ResourceSystem resourceSystem = new();
            ConfigSystem configSystem = new();
            InventorySystem inventorySystem = new();

            try
            {
                resourceSystem.OnInit();
                configSystem.OnInit();
                inventorySystem.OnInit();

                Task initialization = inventorySystem.Initialization;
                while (!initialization.IsCompleted)
                {
                    yield return null;
                }

                Assert.That(initialization.IsFaulted, Is.False, initialization.Exception?.ToString());
                Assert.That(initialization.IsCanceled, Is.False);
                Assert.That(inventorySystem.IsReady, Is.True);
                Assert.That(inventorySystem.Capacity, Is.EqualTo(80));
                Assert.That(inventorySystem.OccupiedSlots, Is.EqualTo(60));
                Assert.That(inventorySystem.RemainingSlots, Is.EqualTo(20));
                Assert.That(inventorySystem.GetItems(), Has.Count.EqualTo(60));
                Assert.That(
                    inventorySystem.GetItems(ItemCategory.Equipment),
                    Has.Count.EqualTo(16));

                IAssetService assetService = ServiceLocator.Get<IAssetService>();
                foreach (InventoryItem item in inventorySystem.GetItems())
                {
                    Assert.That(inventorySystem.TryGetConfig(item.ConfigId, out ItemConfig config), Is.True);
                    Task<Sprite> iconLoading = assetService.LoadAssetAsync<Sprite>(config.IconKey);
                    while (!iconLoading.IsCompleted)
                    {
                        yield return null;
                    }

                    Assert.That(iconLoading.IsFaulted, Is.False, iconLoading.Exception?.ToString());
                    Assert.That(iconLoading.Result, Is.Not.Null, config.IconKey);
                }
            }
            finally
            {
                inventorySystem.OnShutdown();
                configSystem.OnShutdown();
                resourceSystem.OnShutdown();
                ServiceLocator.Clear();
            }
        }
    }
}
#endif
