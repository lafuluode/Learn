using System;
using System.Collections;
using System.Collections.Generic;
using Game.Framework.Core;
using Game.Framework.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 笔试所需的背包窗口：列表、分类切换和点击反馈。
    /// 格子对象会复用，切换分类时不会反复销毁和创建。
    /// </summary>
    public sealed class InventoryView : MonoBehaviour
    {
        public const int PageSize = 28;
        private const int CellsPerFrame = 7;

        [Serializable]
        private sealed class CategoryTab
        {
            public ItemCategory Category = ItemCategory.All;
            public Button Button;
        }

        [SerializeField] private Transform contentRoot;
        [SerializeField] private Transform categoryRoot;
        [SerializeField] private InventoryCellView cellPrefab;
        [SerializeField] private CategoryTab[] categoryTabs;
        [SerializeField] private GameObject emptyState;
        [Header("Pagination")]
        [SerializeField] private Button previousPageButton;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private TMP_Text pageLabel;
        [SerializeField] private TMP_Text capacityLabel;
        [Header("Details")]
        [SerializeField] private InventoryDetailView detailView;

        private readonly List<InventoryCellView> cellPool = new();
        private readonly List<UnityAction> tabActions = new();

        private IInventoryService inventoryService;
        private IAssetService assetService;
        private ItemCategory currentCategory = ItemCategory.All;
        private int currentPage;
        private bool pageControlsBound;
        private Coroutine refreshCoroutine;
        private int refreshVersion;

        private static readonly ItemCategory[] DefaultCategories =
        {
            ItemCategory.All,
            ItemCategory.Equipment,
            ItemCategory.Consumable,
            ItemCategory.Material,
            ItemCategory.Fragment,
            ItemCategory.Other,
        };

        private void Awake()
        {
            PrepareView();
        }

        private IEnumerator Start()
        {
            // Addressables 加载场景时再兜底检查一次，避免场景引用尚未准备好的边缘情况。
            PrepareView();
            if (contentRoot == null || cellPrefab == null)
            {
                Debug.LogError("[InventoryView] 找不到物品列表或物品格模板。", this);
                yield break;
            }

            // GameEntry 在 Start 中初始化系统，因此先等待服务注册，再等待配置异步加载。
            while (!ServiceLocator.TryGet(out inventoryService))
            {
                yield return null;
            }

            while (!inventoryService.Initialization.IsCompleted)
            {
                yield return null;
            }

            if (inventoryService.Initialization.IsCanceled)
            {
                Debug.LogWarning("[InventoryView] 背包初始化已取消。", this);
                yield break;
            }

            if (inventoryService.Initialization.IsFaulted)
            {
                Exception exception = inventoryService.Initialization.Exception?.GetBaseException();
                Debug.LogError($"[InventoryView] 背包初始化失败：{exception?.Message}", this);
                yield break;
            }

            ServiceLocator.TryGet(out assetService);
            BindCategoryTabs();
            BindPageControls();
            inventoryService.Changed += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            CancelRefresh();

            if (inventoryService != null)
            {
                inventoryService.Changed -= Refresh;
            }

            UnbindCategoryTabs();
            UnbindPageControls();
        }

        public void SelectCategory(ItemCategory category)
        {
            if (currentCategory == category && inventoryService != null)
            {
                UpdateTabVisuals();
                return;
            }

            currentCategory = category;
            currentPage = 0;
            UpdateTabVisuals();
            Refresh();
        }

        public void PreviousPage()
        {
            if (currentPage <= 0)
            {
                return;
            }

            currentPage--;
            Refresh();
        }

        public void NextPage()
        {
            if (inventoryService == null || !inventoryService.IsReady)
            {
                return;
            }

            int pageCount = GetPageCount(inventoryService.GetItems(currentCategory).Count);
            if (currentPage >= pageCount - 1)
            {
                return;
            }

            currentPage++;
            Refresh();
        }

        public void Refresh()
        {
            if (inventoryService == null || !inventoryService.IsReady || contentRoot == null || cellPrefab == null)
            {
                CancelRefresh();
                return;
            }

            CancelRefresh();
            int version = refreshVersion;
            refreshCoroutine = StartCoroutine(RefreshByFrame(version));
        }

        private IEnumerator RefreshByFrame(int version)
        {
            IReadOnlyList<InventoryItem> items = inventoryService.GetItems(currentCategory);
            int pageCount = GetPageCount(items.Count);
            currentPage = Mathf.Clamp(currentPage, 0, pageCount - 1);

            int startIndex = currentPage * PageSize;
            int pageItemCount = Mathf.Min(PageSize, items.Count - startIndex);

            // 先回收上一轮展示，避免切换分类或快速翻页时短暂显示旧数据。
            HideAllCells();
            if (emptyState != null)
            {
                emptyState.SetActive(false);
            }

            UpdatePageControls(pageCount);

            int visibleCount = 0;
            int processedThisFrame = 0;
            for (int i = 0; i < pageItemCount; i++)
            {
                if (version != refreshVersion)
                {
                    yield break;
                }

                InventoryItem item = items[startIndex + i];
                if (inventoryService.TryGetConfig(item.ConfigId, out ItemConfig config))
                {
                    InventoryCellView cell = GetOrCreateCell(visibleCount++);
                    cell.gameObject.SetActive(true);
                    cell.Bind(item, config, assetService, OnItemClicked);
                }

                processedThisFrame++;
                if (processedThisFrame >= CellsPerFrame)
                {
                    processedThisFrame = 0;
                    yield return null;
                }
            }

            // 少于一批或空页时也至少挂起一次，确保协程句柄先完成赋值再进入收尾。
            if (processedThisFrame > 0 || pageItemCount == 0)
            {
                yield return null;
            }

            // UnityEngine.Object 使用重载的 null 判断；可选场景引用不能用 ?. 判断。
            if (emptyState != null)
            {
                emptyState.SetActive(visibleCount == 0);
            }

            if (version == refreshVersion)
            {
                refreshCoroutine = null;
            }
        }

        private InventoryCellView GetOrCreateCell(int index)
        {
            if (index < cellPool.Count)
            {
                return cellPool[index];
            }

            InventoryCellView cell = Instantiate(cellPrefab, contentRoot);
            cellPool.Add(cell);
            return cell;
        }

        private void HideAllCells()
        {
            foreach (InventoryCellView cell in cellPool)
            {
                cell.Clear();
                cell.gameObject.SetActive(false);
            }
        }

        private void CancelRefresh()
        {
            refreshVersion++;
            if (refreshCoroutine == null)
            {
                return;
            }

            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }

        private void AutoWireExistingHierarchy()
        {
            Transform background = transform.Find("Background");
            contentRoot ??= background?.Find("items");
            categoryRoot ??= background?.Find("tips");

            Transform pagination = background?.Find("Pagination");
            if (previousPageButton == null)
            {
                previousPageButton = pagination?.Find("Previous")?.GetComponent<Button>();
            }

            if (nextPageButton == null)
            {
                nextPageButton = pagination?.Find("Next")?.GetComponent<Button>();
            }

            if (pageLabel == null)
            {
                pageLabel = pagination?.Find("PageLabel")?.GetComponent<TMP_Text>();
            }

            if (capacityLabel == null)
            {
                capacityLabel = pagination?.Find("CapacityLabel")?.GetComponent<TMP_Text>();
            }

            if (detailView == null)
            {
                detailView = GetComponentInChildren<InventoryDetailView>(true);
            }

            if (cellPrefab == null && contentRoot != null)
            {
                cellPrefab = contentRoot.GetComponentInChildren<InventoryCellView>(true);
            }

            if ((categoryTabs == null || categoryTabs.Length == 0) && categoryRoot != null)
            {
                Button[] buttons = categoryRoot.GetComponentsInChildren<Button>(true);
                int count = Math.Min(buttons.Length, DefaultCategories.Length);
                categoryTabs = new CategoryTab[count];

                for (int i = 0; i < count; i++)
                {
                    categoryTabs[i] = new CategoryTab
                    {
                        Category = DefaultCategories[i],
                        Button = buttons[i],
                    };
                }
            }
        }

        private void InitializeCellPool()
        {
            if (cellPrefab == null || contentRoot == null || cellPrefab.transform.parent != contentRoot)
            {
                return;
            }

            if (cellPool.Contains(cellPrefab))
            {
                return;
            }

            cellPool.Add(cellPrefab);
            cellPrefab.Clear();
            cellPrefab.gameObject.SetActive(false);
        }

        private void PrepareView()
        {
            AutoWireExistingHierarchy();
            InitializeCellPool();
        }

        private void BindCategoryTabs()
        {
            tabActions.Clear();
            if (categoryTabs == null)
            {
                return;
            }

            foreach (CategoryTab tab in categoryTabs)
            {
                if (tab?.Button == null)
                {
                    tabActions.Add(null);
                    continue;
                }

                ItemCategory category = tab.Category;
                UnityAction action = () => SelectCategory(category);
                tab.Button.onClick.AddListener(action);
                tabActions.Add(action);
            }

            UpdateTabVisuals();
        }

        private void UpdateTabVisuals()
        {
            if (categoryTabs == null)
            {
                return;
            }

            foreach (CategoryTab tab in categoryTabs)
            {
                if (tab?.Button != null)
                {
                    tab.Button.interactable = tab.Category != currentCategory;
                }
            }
        }

        private void UnbindCategoryTabs()
        {
            if (categoryTabs == null)
            {
                return;
            }

            int count = Math.Min(categoryTabs.Length, tabActions.Count);
            for (int i = 0; i < count; i++)
            {
                if (categoryTabs[i]?.Button != null && tabActions[i] != null)
                {
                    categoryTabs[i].Button.onClick.RemoveListener(tabActions[i]);
                }
            }

            tabActions.Clear();
        }

        private void BindPageControls()
        {
            if (pageControlsBound)
            {
                return;
            }

            if (previousPageButton != null)
            {
                previousPageButton.onClick.AddListener(PreviousPage);
            }

            if (nextPageButton != null)
            {
                nextPageButton.onClick.AddListener(NextPage);
            }

            pageControlsBound = true;
        }

        private void UnbindPageControls()
        {
            if (!pageControlsBound)
            {
                return;
            }

            if (previousPageButton != null)
            {
                previousPageButton.onClick.RemoveListener(PreviousPage);
            }

            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveListener(NextPage);
            }

            pageControlsBound = false;
        }

        private void UpdatePageControls(int pageCount)
        {
            if (pageLabel != null)
            {
                pageLabel.text = $"{currentPage + 1} / {pageCount}";
            }

            if (capacityLabel != null)
            {
                capacityLabel.text = $"背包容量：{inventoryService.OccupiedSlots} / {inventoryService.Capacity}";
            }

            if (previousPageButton != null)
            {
                previousPageButton.interactable = currentPage > 0;
            }

            if (nextPageButton != null)
            {
                nextPageButton.interactable = currentPage < pageCount - 1;
            }
        }

        private static int GetPageCount(int itemCount)
        {
            return Mathf.Max(1, Mathf.CeilToInt(itemCount / (float)PageSize));
        }

        private void OnItemClicked(InventoryItem item, ItemConfig config)
        {
            if (detailView != null)
            {
                detailView.Show(item, config, assetService);
            }
            Debug.Log(
                $"[Inventory] 点击物品：configId={config.Id}, instanceId={item.InstanceId}, name={config.Name}");
        }
    }
}
