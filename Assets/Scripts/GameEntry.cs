using Game.Framework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace Game.Framework.Core
{
    public interface IGameSystem
    {
        public int Priority { get; }
        public void OnInit();
        public void OnShutdown();

    }
    public interface IUpdateSystem
    {
        void OnUpdate(float deltaTime);
    }


    public class GameEntry : MonoBehaviour
    {
        private static GameEntry instance;
        public static GameEntry Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("[GameEntry]");
                    instance = go.AddComponent<GameEntry>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }
        private bool isInitialized;
        private bool isShuttingDown;
        List<IGameSystem> gameSystems = new();
        List<IUpdateSystem> updateSystems = new();
        void RegisterSystem(IGameSystem system)
        {
            if (system == null)
            {
                Debug.LogError("Cannot register a null system.");
                return;
            }
            gameSystems.Add(system);
        }


        void RegisterSystems()
        {
            RegisterSystem(new AddressableSystem());
        }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }


        // Start is called before the first frame update
        void Start()
        {
            Init();
        }
        private void Update()
        {
            if (!isInitialized || isShuttingDown)
                return;
            float deltaTime = Time.deltaTime;
            foreach (var system in updateSystems)
            {
                system.OnUpdate(deltaTime);
            }
        }
        private void OnDestroy()
        {
            if (instance != this)
                return;
            Shutdown();
        }

        private void OnApplicationQuit()
        {
            isShuttingDown = true;
            Shutdown();
        }


        private void Init()
        {
            if (isInitialized)
            {
                return;
            }
            RegisterSystems();
            gameSystems.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            foreach (var system in gameSystems)
            {
                system.OnInit();
                //完成updatesystem的注册
                if (system is IUpdateSystem updateSystem)
                {
                    updateSystems.Add(updateSystem);
                }
            }

            isInitialized = true;

            Debug.Log("[GameEntry] 初始化完成");
        }

        private void Shutdown()
        {
            for (int i = gameSystems.Count - 1; i >= 0; i--)
            {
                gameSystems[i].OnShutdown();
            }
            updateSystems.Clear();
            gameSystems.Clear();

            ServiceLocator.Clear();
        }

    }
}