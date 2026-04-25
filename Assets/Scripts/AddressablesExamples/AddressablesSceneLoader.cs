using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Learn.AddressablesExamples
{
    public class AddressablesSceneLoader : MonoBehaviour
    {
        [SerializeField] private AssetReference sceneReference;
        [SerializeField] private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
        [SerializeField] private bool activateOnLoad = true;

        private AsyncOperationHandle<SceneInstance>? _sceneHandle;
        private bool _isLoaded;

        [ContextMenu("Load Scene")]
        public void LoadScene()
        {
            if (_isLoaded)
            {
                Debug.LogWarning("Scene is already loaded.", this);
                return;
            }

            if (sceneReference == null || !sceneReference.RuntimeKeyIsValid())
            {
                Debug.LogWarning("Assign an addressable scene reference first.", this);
                return;
            }

            _sceneHandle = sceneReference.LoadSceneAsync(loadSceneMode, activateOnLoad);
            _sceneHandle.Value.Completed += OnSceneLoaded;
        }

        [ContextMenu("Unload Scene")]
        public void UnloadScene()
        {
            if (!_isLoaded || !_sceneHandle.HasValue)
            {
                return;
            }

            Addressables.UnloadSceneAsync(_sceneHandle.Value, autoReleaseHandle: true);
            _sceneHandle = null;
            _isLoaded = false;
        }

        private void OnDestroy()
        {
            UnloadScene();
        }

        private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load scene: {sceneReference.RuntimeKey}", this);
                return;
            }

            _isLoaded = true;
        }
    }
}
