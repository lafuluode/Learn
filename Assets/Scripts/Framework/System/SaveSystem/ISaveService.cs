namespace Game.Framework.Save
{
    /// <summary>
    /// 存档服务接口
    /// </summary>
    /// <remarks>
    /// 该接口定义了存档系统的基本操作，包括保存、加载、删除和存在性检查能力。
    /// 
    /// 存档服务不直接关系可能具体的序列化格式，也不直接关心底层文件读写细节
    /// 具体的对象与字节转换由序列化层负责
    /// 具体文件读写由存储层负责
    /// </remarks>
    public interface ISaveService
    {
        /// <summary>
        /// 判断指定名称的存档是否存在
        /// </summary>
        /// <param name="saveName">存档名称</param>
        bool HasSave(string saveName);
        /// <summary>
        /// 加载指定名称的存档
        /// </summary>
        /// <typeparam name="T">存档数据类型</typeparam>
        /// <param name="saveName">存档名称</param>
        T Load<T>(string saveName) where T : class,new();

        /// <summary>
        /// 尝试加载指定名称的存档
        /// </summary>
        /// <typeparam name="T">存档数据类型</typeparam>
        /// <param name="saveName">存档名称</param>
        /// <param name="data">输出的存档数据</param>
        bool TryLoad<T>(string saveName, out T data) where T : class,new();

        /// <summary>
        /// 保存指定名称的存档
        /// </summary>
        /// <typeparam name="T">存档数据类型</typeparam>
        /// <param name="saveName">存档名称</param>
        /// <param name="data">存档数据</param>
        void Save<T>(string saveName, T data) where T : class,new();

        /// <summary>
        /// 删除指定名称的存档
        /// </summary>
        /// <param name="saveName">存档名称</param>
        void Delete(string saveName);
    }
}