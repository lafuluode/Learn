using Game.Framework.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Boot
{
    public class BootManager : MonoBehaviour
    {
        [Header("Boot UI")]
        [SerializeField] private BootUI bootUI;

        [Header("Boot Settings")]
        [SerializeField] private bool downloadPreloadAssets = true;

        // Start is called before the first frame update
        async Task Start()
        {
            try
            {
                await RunBootFlowAync();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[BootManager] 启动流程异常: {ex}");
                bootUI?.SetText("启动失败，请重试");
            }
        }
        private async Task RunBootFlowAync()
        {
            Debug.Log("[BootManager] 启动流程开始");

            bootUI?.SetText("初始化游戏...");
            bootUI?.SetProgress(0.05f);
            //确保GameEntry存在并完成系统注册
            _ = GameEntry.Instance;

            bootUI?.SetText("获取资源服务...");
            bootUI?.SetProgress(0.1f);

            var assertService = ServiceLocator.Get<IAssetService>();
            bootUI?.SetText("初始化资源系统...");
            bootUI?.SetProgress(0.2f);

            //await assertService.InstantiateAsync()
        }
        private async Task DownloadPreloadAssetsAsync(IAssetService assetService)
        {
            //long downloadSize = await assetService.Get
        }

    }
}