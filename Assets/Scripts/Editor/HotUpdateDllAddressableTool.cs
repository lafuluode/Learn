using System.IO;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class HotUpdateDllAddressableTool
{
    private const string HotUpdateAssemblyName = "HotUpdate";
    private const string TargetAssetDirectory = "Assets/GameAssets/HotUpdateDlls";
    private const string AddressableAddress = "HotUpdate.dll.bytes";
    private const string AddressableLabel = "Preload";

    [MenuItem("Tools/Learn MMO/HotUpdate/Compile And Copy DLL To Addressables")]
    [MenuItem("Tools/Learn MMO/HotUpdate/Copy DLL To Addressables")]
    public static void CopyDllToAddressables()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        if (!CompileHotUpdateDll(buildTarget))
        {
            return;
        }

        string buildTargetName = buildTarget.ToString();
        string sourceFullPath = Path.Combine(
            projectRoot,
            "HybridCLRData",
            "HotUpdateDlls",
            buildTargetName,
            HotUpdateAssemblyName + ".dll");

        string targetAssetPath = TargetAssetDirectory + "/" + HotUpdateAssemblyName + ".dll.bytes";
        string targetFullPath = Path.GetFullPath(Path.Combine(projectRoot, targetAssetPath));

        if (!File.Exists(sourceFullPath))
        {
            Debug.LogError(
                "[HotUpdateDllAddressableTool] 编译完成后仍找不到热更 DLL:\n" +
                sourceFullPath +
                "\n\n请检查 HybridCLR Settings 中的热更程序集配置，以及当前 Build Target 是否正确。");
            return;
        }

        string targetFullDirectory = Path.GetDirectoryName(targetFullPath);
        if (!Directory.Exists(targetFullDirectory))
        {
            Directory.CreateDirectory(targetFullDirectory);
        }

        File.Copy(sourceFullPath, targetFullPath, true);

        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(targetAssetPath, ImportAssetOptions.ForceUpdate);

        if (!MarkAsAddressable(targetAssetPath))
        {
            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "[HotUpdateDllAddressableTool] 热更 DLL 已编译、复制并加入 Addressables:\n" +
            "Source: " + sourceFullPath + "\n" +
            "Target: " + targetAssetPath + "\n" +
            "Address: " + AddressableAddress + "\n" +
            "Label: " + AddressableLabel);
    }

    private static bool CompileHotUpdateDll(BuildTarget buildTarget)
    {
        Debug.Log("[HotUpdateDllAddressableTool] 开始编译 HybridCLR 热更 DLL，BuildTarget: " + buildTarget);

        try
        {
            CompileDllCommand.CompileDll(buildTarget, EditorUserBuildSettings.development);
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
            Debug.LogError("[HotUpdateDllAddressableTool] HybridCLR 热更 DLL 编译失败，请先处理 Console 中的编译错误。 ");
            return false;
        }

        AssetDatabase.Refresh();
        Debug.Log("[HotUpdateDllAddressableTool] HybridCLR 热更 DLL 编译完成，BuildTarget: " + buildTarget);
        return true;
    }

    private static bool MarkAsAddressable(string assetPath)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError(
                "[HotUpdateDllAddressableTool] AddressableAssetSettings 不存在。" +
                "请先打开 Window/Asset Management/Addressables/Groups 初始化 Addressables。");
            return false;
        }

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogError("[HotUpdateDllAddressableTool] 无法获取资源 GUID: " + assetPath);
            return false;
        }

        AddressableAssetGroup group = settings.DefaultGroup;
        if (group == null)
        {
            Debug.LogError("[HotUpdateDllAddressableTool] Addressables DefaultGroup 不存在。");
            return false;
        }

        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
        entry.address = AddressableAddress;

        settings.AddLabel(AddressableLabel);
        entry.SetLabel(AddressableLabel, true, true);

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
        Debug.Log("[HotUpdateDllAddressableTool] Addressables 设置完成: " + assetPath);
        return true;
    }
}

