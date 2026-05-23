using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Serialization
{
    public class MessagePackDataSerializer : IDataSerializer
    {

        public byte[] Serialize<T>(T data)
        {
            throw new System.NotImplementedException();
        }
        public T Deserialize<T>(byte[] data)
        {
            throw new System.NotImplementedException();
        }

    }
}