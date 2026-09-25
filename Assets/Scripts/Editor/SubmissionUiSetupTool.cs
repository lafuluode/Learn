using Game.Framework.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SubmissionUiSetupTool
{
    private const string MenuPath = "Tools/Learn MMO/Submission/Ensure Exit Button";
    private const string MainMenuScenePath = "Assets/Scenes/MainMenuScene.unity";

    [MenuItem(MenuPath)]
    private static void EnsureExitButton()
    {
        if (EditorSceneManager.GetActiveScene().path != MainMenuScenePath)
        {
            EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
        }

        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            throw new System.InvalidOperationException("MainMenuScene 中找不到 Canvas。");
        }

        Transform existing = canvasObject.transform.Find("QuitGameButton");
        GameObject buttonObject = existing != null
            ? existing.gameObject
            : CreateButtonObject(canvasObject.transform);

        ConfigureButton(buttonObject);
        ConfigureLabel(buttonObject.transform);

        buttonObject.transform.SetAsLastSibling();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = buttonObject;

        Debug.Log("[SubmissionUiSetupTool] 已创建或更新主菜单退出按钮。");
    }

    private static GameObject CreateButtonObject(Transform parent)
    {
        var buttonObject = new GameObject(
            "QuitGameButton",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button),
            typeof(Outline),
            typeof(GameExitButton));

        buttonObject.layer = LayerMask.NameToLayer("UI");
        buttonObject.transform.SetParent(parent, false);
        return buttonObject;
    }

    private static void ConfigureButton(GameObject buttonObject)
    {
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-24f, -24f);
        rect.sizeDelta = new Vector2(176f, 52f);
        rect.localScale = Vector3.one;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.11f, 0.16f, 0.25f, 0.96f);

        Outline outline = buttonObject.GetComponent<Outline>();
        outline.effectColor = new Color(0.25f, 0.78f, 1f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.72f, 0.92f, 1f, 1f);
        colors.pressedColor = new Color(1f, 0.72f, 0.3f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
    }

    private static void ConfigureLabel(Transform buttonTransform)
    {
        Transform existing = buttonTransform.Find("Label");
        TextMeshProUGUI label;
        if (existing == null)
        {
            var textObject = new GameObject(
                "Label",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI));

            textObject.layer = LayerMask.NameToLayer("UI");
            textObject.transform.SetParent(buttonTransform, false);
            label = textObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            label = existing.GetComponent<TextMeshProUGUI>();
        }

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        label.text = "退出游戏  Esc";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 22f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
        label.raycastTarget = false;

        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/Art/Font/ZhanKU SDF.asset");
        if (font != null)
        {
            label.font = font;
        }
    }
}
