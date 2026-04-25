using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Learn.AddressablesExamples
{
    public class AddressablesLabelPreloadExample : MonoBehaviour
    {
        [SerializeField] private string labelName = "preload";
        [SerializeField] private bool preloadOnStart = true;

        private AsyncOperationHandle<IList<GameObject>>? _preloadHandle;

        private void Start()
        {
            if (preloadOnStart)
            {
                PreloadByLabel();
            }
        }

        [ContextMenu("Preload By Label")]
        public void PreloadByLabel()
        {
            if (string.IsNullOrWhiteSpace(labelName))
            {
                Debug.LogWarning("Label name is empty.", this);
                return;
            }

            ReleasePreload();

            _preloadHandle = Addressables.LoadAssetsAsync<GameObject>(labelName, null);
            _preloadHandle.Value.Completed += OnPreloadCompleted;
        }

        [ContextMenu("Release Preload")]
        public void ReleasePreload()
        {
            if (_preloadHandle.HasValue)
            {
                Addressables.Release(_preloadHandle.Value);
                _preloadHandle = null;
            }
        }

        private void OnDestroy()
        {
            ReleasePreload();
        }

        private void OnPreloadCompleted(AsyncOperationHandle<IList<GameObject>> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to preload assets with label: {labelName}", this);
                return;
            }

            Debug.Log($"Preloaded {handle.Result.Count} GameObject assets with label '{labelName}'.", this);
        }
    }
}
