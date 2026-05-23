using System;
using System.Text;
using UnityEngine;

namespace Game.Framework.Serialization
{
    /// <summary>
    /// 基于unity JsonUtility 的 Json 序列化器
    /// </summary>
    /// <remarks>
    /// JsonUtility 是 Unity 内置的 Json 序列化工具
    /// 对复杂类型支持有限，例如不支持 Dictionary、接口类型等
    /// 复杂配表或存档结构可替换为Netonsoft.Json、MessagePack 或 Protobuf
    /// </remarks>
    public class UnityJsonDataSerializer : IDataSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data)); 
            }   
            string json = JsonUtility.ToJson(data);
            return Encoding.UTF8.GetBytes(json);
        }
        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            string json = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<T>(json);
        }
    }
}