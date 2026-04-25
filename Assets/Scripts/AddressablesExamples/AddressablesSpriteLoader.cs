using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Learn.AddressablesExamples
{
    public class AddressablesSpriteLoader : MonoBehaviour
    {
        [Header("Addressables")]
        [SerializeField] private AssetReferenceSprite spriteReference;
        [SerializeField] private bool loadOnStart = true;

        [Header("Targets")]
        [SerializeField] private Image targetImage;
        [SerializeField] private SpriteRenderer targetRenderer;

        private AsyncOperationHandle<Sprite>? _loadHandle;

        private void Start()
        {
            if (loadOnStart)
            {
                LoadSprite();
            }
        }

        [ContextMenu("Load Sprite")]
        public void LoadSprite()
        {
            if (spriteReference == null || !spriteReference.RuntimeKeyIsValid())
            {
                Debug.LogWarning("Assign an addressable sprite reference first.", this);
                return;
            }

            ReleaseSprite();

            _loadHandle = spriteReference.LoadAssetAsync<Sprite>();
            _loadHandle.Value.Completed += OnSpriteLoaded;
        }

        [ContextMenu("Release Sprite")]
        public void ReleaseSprite()
        {
            if (_loadHandle.HasValue)
            {
                Addressables.Release(_loadHandle.Value);
                _loadHandle = null;
            }

            if (targetImage != null)
            {
                targetImage.sprite = null;
            }

            if (targetRenderer != null)
            {
                targetRenderer.sprite = null;
            }
        }

        private void OnDestroy()
        {
            ReleaseSprite();
        }

        private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load sprite: {spriteReference.RuntimeKey}", this);
                return;
            }

            if (targetImage != null)
            {
                targetImage.sprite = handle.Result;
                targetImage.SetNativeSize();
            }

            if (targetRenderer != null)
            {
                targetRenderer.sprite = handle.Result;
            }
        }
    }
}
