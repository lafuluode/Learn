using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Serialization
{
    /// <summary>
    /// 通用数据序列化接口
    /// </summary>
    /// /// <remarks>
    /// 该接口只负责“对象”和“字节数据”之间的转换，
    /// 不关心数据来自哪里，也不关心数据最终保存到哪里。
    /// 
    /// 例如：
    /// 配置表系统可以使用它将配置文件内容反序列化为配置对象；
    /// 存档系统可以使用它将存档对象序列化为字节数据，再交给存储层保存。
    /// </remarks>
    public interface IDataSerializer
    {
        /// <summary>
        /// 将指定对象序列化为字节数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">要序列化的对象</param>
        /// <returns>序列化后的字节数组</returns>
        byte[] Serialize<T>(T data);
        
        /// <summary>
        /// 将字节数据反序列化为指定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">要反序列化的字节数组</param>
        /// <returns>反序列化后的对象</returns>
        T Deserialize<T>(byte[] data);

    }
}
