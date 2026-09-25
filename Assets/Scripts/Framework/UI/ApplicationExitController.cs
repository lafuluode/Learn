using UnityEngine;

namespace Game.Framework.UI
{
    /// <summary>
    /// Standalone Player 的窗口与退出控制。
    /// 不依赖具体场景，并在场景切换时持续存在。
    /// </summary>
    public sealed class ApplicationExitController : MonoBehaviour
    {
        private const int DefaultWindowWidth = 1280;
        private const int DefaultWindowHeight = 720;

        private static ApplicationExitController instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(DefaultWindowWidth, DefaultWindowHeight, FullScreenMode.Windowed);
#endif

            if (instance != null)
            {
                return;
            }

            var host = new GameObject("[ApplicationExitController]");
            instance = host.AddComponent<ApplicationExitController>();
            DontDestroyOnLoad(host);
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("[ApplicationExitController] Player 中将退出游戏；Editor 下忽略 Application.Quit。");
#else
            Application.Quit();
#endif
        }
    }
}
