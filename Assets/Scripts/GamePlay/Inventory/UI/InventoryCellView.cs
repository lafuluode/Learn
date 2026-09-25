using System;
using Game.Framework.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 单个物品格子。只显示数据并上报点击，不负责筛选和背包规则。
    /// </summary>
    public sealed class InventoryCellView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text countLabel;
        [SerializeField] private Sprite fallbackIcon;

        private InventoryItem item;
        private ItemConfig config;
        private Action<InventoryItem, ItemConfig> clickHandler;
        private int bindVersion;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (icon == null)
            {
                Transform iconTransform = transform.Find("Icon");
                icon = iconTransform != null
                    ? iconTransform.GetComponent<Image>()
                    : GetComponent<Image>();
            }

            if (nameLabel == null)
            {
                nameLabel = transform.Find("Name")?.GetComponent<TMP_Text>();
            }

            if (countLabel == null)
            {
                countLabel = transform.Find("Count")?.GetComponent<TMP_Text>();
            }

            button?.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            button?.onClick.RemoveListener(OnClick);
        }

        public void Bind(
            InventoryItem inventoryItem,
            ItemConfig itemConfig,
            IAssetService assetService,
            Action<InventoryItem, ItemConfig> onClick)
        {
            item = inventoryItem ?? throw new ArgumentNullException(nameof(inventoryItem));
            config = itemConfig ?? throw new ArgumentNullException(nameof(itemConfig));
            clickHandler = onClick;

            if (nameLabel != null)
            {
                nameLabel.text = config.Name;
            }

            if (countLabel != null)
            {
                // 当前采用非堆叠规则：一件物品占一个格子，不显示数量角标。
                countLabel.text = string.Empty;
                countLabel.gameObject.SetActive(false);
            }

            bindVersion++;
            LoadIconAsync(assetService, config.IconKey, bindVersion);
        }

        public void Clear()
        {
            bindVersion++;
            item = null;
            config = null;
            clickHandler = null;

            if (nameLabel != null)
            {
                nameLabel.text = string.Empty;
            }

            if (countLabel != null)
            {
                countLabel.text = string.Empty;
                countLabel.gameObject.SetActive(false);
            }

            SetIcon(fallbackIcon);
        }

        private async void LoadIconAsync(IAssetService assetService, string iconKey, int version)
        {
            if (assetService == null || string.IsNullOrWhiteSpace(iconKey))
            {
                SetIcon(fallbackIcon);
                return;
            }

            try
            {
                Sprite loadedIcon = await assetService.LoadAssetAsync<Sprite>(iconKey);
                if (this == null || version != bindVersion)
                {
                    return;
                }

                SetIcon(loadedIcon != null ? loadedIcon : fallbackIcon);
            }
            catch (Exception exception)
            {
                if (this == null || version != bindVersion)
                {
                    return;
                }

                SetIcon(fallbackIcon);
                Debug.LogWarning($"[InventoryCellView] 图标加载失败：{iconKey}\n{exception.Message}");
            }
        }

        private void SetIcon(Sprite sprite)
        {
            if (icon == null)
            {
                return;
            }

            icon.sprite = sprite;
            icon.enabled = sprite != null;
        }

        private void OnClick()
        {
            if (item != null && config != null)
            {
                clickHandler?.Invoke(item, config);
            }
        }
    }
}
