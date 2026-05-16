using UnityEngine;

namespace Game.Framework.HotUpdate
{
    [CreateAssetMenu(fileName = "HotUpdateConfig", menuName = "Game/HotUpdate/ScriptHotUpdateConfig")]
    public class ScriptHotUpdateConfig : ScriptableObject
    {
        [Header("HybridCLR 热更新程序集资源 Key")]
        public string hybridClrAssemblyKey = "HotUpdate.dll.bytes";

        [Header("HybridCLR 热更入口类型完整名")]
        public string hybridClrEntryTypeName = "HotUpdate.HotUpdateEntry";

        [Header("HybridCLR 热更入口方法名")]
        public string hybridClrEntryMethodName = "Start";
    }
}