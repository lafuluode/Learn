using UnityEngine;
using UnityEngine.UI;

namespace Game.Framework.UI
{
    /// <summary>
    /// 将普通 uGUI Button 连接到统一的退出流程。
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class GameExitButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ApplicationExitController.QuitGame);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(ApplicationExitController.QuitGame);
            }
        }
    }
}
