namespace Game.Framework.Storage
{
    /// <summary>
    /// 文件存储接口
    /// </summary>
    /// <remarks>
    /// 它不关心文件内容是什么格式，也不关心这些数据属于哪个系统
    /// </remarks>
    public interface IFileStorage 
    {
        /// <summary>
        /// 判断指定相对路径的文件是否存在
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        bool Exists(string relativePath);

        /// <summary>
        /// 读取指定相对路径文件的全部字节数据
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>文件的字节数组</returns>
        byte[] Read(string relativePath);

        /// <summary>
        /// 将字节数据写入指定相对路径的文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="bytes">要写入的字节数据</param>
        void Write(string relativePath, byte[] bytes);

        /// <summary>
        /// 删除指定相对路径的文件
        /// </summary>
        /// <param name="relativePath">相对于存储根目录的文件路径</param>
        void Delete(string relativePath);
    }
}