using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Learn.AddressablesExamples
{
    public class AddressablesPrefabSpawner : MonoBehaviour
    {
        [Header("Addressables")]
        [SerializeField] private AssetReferenceGameObject prefabReference;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool spawnOnStart = true;

        private AsyncOperationHandle<GameObject>? _spawnHandle;
        private GameObject _spawnedInstance;

        private void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            if (_spawnedInstance != null)
            {
                Debug.LogWarning("An instance is already active. Release it before spawning again.", this);
                return;
            }

            if (prefabReference == null || !prefabReference.RuntimeKeyIsValid())
            {
                Debug.LogWarning("Assign an addressable prefab reference first.", this);
                return;
            }

            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            _spawnHandle = prefabReference.InstantiateAsync(position, rotation);
            _spawnHandle.Value.Completed += OnSpawnCompleted;
        }

        [ContextMenu("Release Spawned Instance")]
        public void ReleaseSpawnedInstance()
        {
            if (_spawnedInstance == null)
            {
                return;
            }

            Addressables.ReleaseInstance(_spawnedInstance);
            _spawnedInstance = null;
            _spawnHandle = null;
        }

        private void OnDestroy()
        {
            ReleaseSpawnedInstance();
        }

        private void OnSpawnCompleted(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to spawn addressable prefab: {prefabReference.RuntimeKey}", this);
                return;
            }

            _spawnedInstance = handle.Result;
        }
    }
}
