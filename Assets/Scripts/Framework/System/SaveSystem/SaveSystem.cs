using Game.Framework.Save;
using Game.Framework.Serialization;
using Game.Framework.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Core
{
    /// <summary>
    /// 存档系统
    /// </summary>
    /// <remarks>
    /// SaveSystem负责管理存档服务的生命周期
    /// </remarks>
    public class SaveSystem : IGameSystem
    {
        private ISaveService saveService;

        public int Priority => 300;

        public void OnInit()
        {
            IDataSerializer serializer = new UnityJsonDataSerializer();

            IFileStorage fileStorage = new LocalFileStorage(Application.persistentDataPath);

            saveService = new SaveService(serializer, fileStorage);

            ServiceLocator.Register<ISaveService>(saveService);

            Debug.Log("SaveSystem initialized.");
        }

        public void OnShutdown()
        {
            ServiceLocator.Unregister<ISaveService>();
            saveService = null;
            Debug.Log("[SaveSystem] 存档系统关闭完成");
        }
    }
}