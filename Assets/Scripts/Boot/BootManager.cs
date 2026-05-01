using Game.Framework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Boot
{
    /// <summary>
    /// 启动流程管理器
    /// 职责：
    /// 1.读取启动配置（BootConfig），并验证配置的有效性
    /// 2.获取资源服务
    /// 3.下载预加载资源
    /// 4.加载主菜单场景
    /// 5.处理启动失败和重试
    /// 注意：
    /// BootManager 不负责创建资源系统
    /// BootManager 不直接依赖 Addressable。
    /// BootManager 只依赖 IAssetService 接口抽象
    /// </summary>
    public class BootManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private BootConfig bootConfig;

        [Header("View")]
        [SerializeField] private BootLoadingView loadingView;

        private CancellationTokenSource cancellationTokenSource;
        private bool isBooting;
        private void Start()
        {
            StartBoot();
        }
        private async void StartBoot()
        {
            if (isBooting)
            {
                return;
            }

            isBooting = true;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await BootAsync(cancellationTokenSource.Token);
            }
            catch(OperationCanceledException)
            {

                Debug.Log("Boot flow was cancelled.");
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
                ShowBootError("启动失败，请检查网络或资源配置");
            }
            finally
            {
                isBooting = false;
            }
        }

        /// <summary>
        /// 真正的启动流程
        /// 
        /// 职责：
        /// 1、 校验 BootConfig 和 View 配置
        /// 2、 获取资源服务 IAssetService
        /// 3、 下载预加载资源
        /// 4、 加载主菜单场景
        /// 
        /// 这个方法只负责编排流程，不关心资源服务底层如何实现。
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// 取消令牌。
        /// 当 BootManager 被销毁或者启动流程需要中断时，用它来停止后续流程。
        /// <returns></returns>
        private async Task BootAsync(CancellationToken cancellationToken)
        {
            ValidateConfig();

            loadingView?.HideError();

            SetProgress("准备启动...", 0f);
            var assetService = ResolveAssetService();
            cancellationToken.ThrowIfCancellationRequested();

            SetProgress("检查预加载资源...",0.1f);

            var downloadProgress = new Progress<float>(value =>
            {
                float mappedProgress = Mathf.Lerp(0.1f, 0.8f,value);
                SetProgress("下载预加载资源...", mappedProgress);
            });

            await assetService.DownloadDependenciesAsync(
                bootConfig.PreloadGroupKey,
                downloadProgress
                );
            cancellationToken.ThrowIfCancellationRequested();

            SetProgress("加载主菜单...", 0.9f);

            await assetService.LoadSceneAsync(bootConfig.MainMenuSceneKey);
            cancellationToken.ThrowIfCancellationRequested();
            SetProgress("启动完成", 1f);
        }

        /// <summary>
        /// 获取资源服务实例
        /// </summary>
        /// <returns></returns>
        private IAssetService ResolveAssetService()
        {
            if (ServiceLocator.TryGet<IAssetService>(out var assetService))
            {
                return assetService;
            }
            else
            {
                throw new InvalidOperationException("IAssetService 没有被注册在 ServiceLocator 中.");
            }
        }
        /// <summary>
        /// 校验启动配置是否合法
        /// 
        /// 职责：
        /// 1、检查BootConfig是否为空
        /// 2、检查预加载资源组Key是否为空
        /// 3、检查主菜单场景Key是否为空
        /// 4、检查 LoadingView 缺失，只输出警告
        /// 
        /// 关键配置缺失时直接抛异常，
        /// 非关键配置，比如 loadingView 缺失，只输出警告。
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ValidateConfig()
        {
            if(bootConfig == null)
            {
                throw new InvalidOperationException("BootConfig is not assigned in BootManager.");
            }
            if(string.IsNullOrWhiteSpace(bootConfig.PreloadGroupKey))
            {
                throw new InvalidOperationException("BootConfig.PreloadGroupKey is empty.");          
            }
            if (string.IsNullOrWhiteSpace(bootConfig.MainMenuSceneKey))
            {
                throw new InvalidOperationException("BootConfig.MainMenuSceneKey is empty.");
            }
            if(loadingView == null)
            {
                Debug.LogWarning("BootLoadingView is not assigned in BootManager. Progress will not be displayed.");
            }
        }
        private void SetProgress(string message, float progress)
        {
            if (loadingView != null)
            {
                loadingView.SetMessage(message);
                loadingView.SetProgress(progress);
            }
        }

        /// <summary>
        /// 显示启动失败界面
        /// 
        /// 职责：
        /// 1. 把错误信息交给BootLoadingView显示
        /// 2. 绑定充实按钮回调
        /// 
        /// 当用户点击重试按钮时，会重新调用startBoot
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowBootError(string errorMessage)
        {
            if (loadingView != null)
            {
                loadingView.ShowError(errorMessage, StartBoot);
            }
        }

        /// <summary>
        /// 当BootManager被销毁时，取消正在进行的启动流程，释放资源，并防止内存泄漏
        /// 
        /// 职责：
        /// 1、取消尚未完成的启动异步流程
        /// 2、释放 CancellationTokenSource 资源
        /// 3、防止异步流程在对象销毁后继续操作 UI 或切换场景
        /// </summary>
        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }
}