
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Framework.Serialization
{
    public class NewtonsoftJsonDataSerializer : IDataSerializer
    {
        public T Deserialize<T>(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Serialize<T>(T data)
        {
            throw new System.NotImplementedException();
        }
    }
}