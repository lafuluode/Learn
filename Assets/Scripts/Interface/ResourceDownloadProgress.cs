using UnityEngine.UIElements;

namespace Game.Framework.Core
{
    /// <summary>
    /// 资源下载进度信息
    /// 用于 Boot UI 显示下载进度、下载大小和状态
    /// </summary>
    public struct ResourceDownloadProgress
    {
        /// <summary>
        /// 已加载字节数
        /// </summary>
        public long DownloadedBytes;

        /// <summary>
        /// 总字节数
        /// </summary>
        public long TotalBytes;

        /// <summary>
        /// 下载进度，范围 0 到 1；
        /// </summary>
        public float Percent;

        /// <summary>
        /// 当前状态描述
        /// 例如：正在检查更新、正在下载资源、下载完成
        /// </summary>
        public string Message;

        public ResourceDownloadProgress(
            long downloadedBytes,
            long totalBytes,
            float percent,
            string message
            )
        {
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
            Percent = percent;
            Message = message;
        }
    }
}