using System;
using Game.Framework.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.Inventory
{
    /// <summary>
    /// 物品详情浮层。只负责展示选中物品，不修改背包数据。
    /// </summary>
    public sealed class InventoryDetailView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text descriptionLabel;
        [SerializeField] private TMP_Text idLabel;
        [SerializeField] private Button closeButton;
        [SerializeField] private Sprite fallbackIcon;

        private int showVersion;

        private void Awake()
        {
            AutoWire();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }

        public void Show(InventoryItem item, ItemConfig config, IAssetService assetService)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            AutoWire();

            if (nameLabel != null)
            {
                nameLabel.text = config.Name;
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.text = config.Description;
            }

            if (idLabel != null)
            {
                idLabel.text = $"配置 ID：{config.Id}    实例 ID：{item.InstanceId}";
            }

            showVersion++;
            LoadIconAsync(assetService, config.IconKey, showVersion);
        }

        public void Hide()
        {
            showVersion++;
            gameObject.SetActive(false);
        }

        private void AutoWire()
        {
            Transform panel = transform.Find("Panel");
            if (panel == null)
            {
                return;
            }

            if (icon == null)
            {
                icon = panel.Find("Icon")?.GetComponent<Image>();
            }

            if (nameLabel == null)
            {
                nameLabel = panel.Find("Name")?.GetComponent<TMP_Text>();
            }

            if (descriptionLabel == null)
            {
                descriptionLabel = panel.Find("Description")?.GetComponent<TMP_Text>();
            }

            if (idLabel == null)
            {
                idLabel = panel.Find("Id")?.GetComponent<TMP_Text>();
            }

            if (closeButton == null)
            {
                closeButton = panel.Find("Close")?.GetComponent<Button>();
            }
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
                if (this == null || version != showVersion)
                {
                    return;
                }

                SetIcon(loadedIcon != null ? loadedIcon : fallbackIcon);
            }
            catch (Exception exception)
            {
                if (this == null || version != showVersion)
                {
                    return;
                }

                SetIcon(fallbackIcon);
                Debug.LogWarning($"[InventoryDetailView] 图标加载失败：{iconKey}\n{exception.Message}", this);
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
    }
}
