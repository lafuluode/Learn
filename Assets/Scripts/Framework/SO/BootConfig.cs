using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BootConfig", menuName = "Game/Boot/Boot Config")]
public class BootConfig : ScriptableObject
{
    [Header("Resource")]
    [SerializeField] private string preloadGroupKey;
    [SerializeField] private string mainMenuSceneKey;

    [Header("Boot Flow")]
    [SerializeField] private bool enableCatalogUpdate = true;
    [SerializeField] private bool enablePreload = true;
    [SerializeField] private bool enableScriptHotUpdate = true;

    public string PreloadGroupKey => preloadGroupKey;
    public string MainMenuSceneKey => mainMenuSceneKey;

    public bool EnableCatalogUpdate => enableCatalogUpdate;
    public bool EnablePreload => enablePreload;
    public bool EnableScriptHotUpdate => enableScriptHotUpdate;
}
