using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BootConfig", menuName = "Game/Boot/Boot Config")]
public class BootConfig : ScriptableObject
{
    [SerializeField] private string preloadGroupKey;
    [SerializeField] private string mainMenuSceneKey;

    public string PreloadGroupKey => preloadGroupKey;
    public string MainMenuSceneKey => mainMenuSceneKey;
}
