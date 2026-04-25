using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Learn.AddressablesExamples.Editor
{
    public static class AddressablesExampleSetup
    {
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";
        private const string LessonScenePath = "Assets/Scenes/AddressablesLessonScene.unity";
        private const string PrimaryPrefabPath = "Assets/Art/Tilemap/Norules/Castle(NoRule).prefab";
        private const string SecondaryPrefabPath = "Assets/Art/Tilemap/CastleTile/CastleTile(Rule).prefab";

        [MenuItem("Tools/Addressables/Setup Lesson Examples")]
        public static void SetupLessonExamples()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null || settings.DefaultGroup == null)
            {
                Debug.LogError("Addressables settings are not available.");
                return;
            }

            string prefabPath = File.Exists(PrimaryPrefabPath) ? PrimaryPrefabPath : SecondaryPrefabPath;
            string spritePath = FindFirstSpritePath();

            if (!File.Exists(prefabPath))
            {
                Debug.LogError("No prefab asset was found for the Addressables lesson.");
                return;
            }

            if (string.IsNullOrEmpty(spritePath))
            {
                Debug.LogError("No Sprite asset was found for the Addressables lesson.");
                return;
            }

            if (!File.Exists(LessonScenePath))
            {
                Debug.LogError($"Lesson scene is missing: {LessonScenePath}");
                return;
            }

            AddressableAssetEntry prefabEntry = EnsureEntry(settings, prefabPath, "lesson/prefab/castle", "preload");

            if (File.Exists(SecondaryPrefabPath))
            {
                EnsureEntry(settings, SecondaryPrefabPath, "lesson/prefab/castle-rule", "preload");
            }

            AddressableAssetEntry spriteEntry = EnsureEntry(settings, spritePath, "lesson/sprite/demo");
            AddressableAssetEntry sceneEntry = EnsureEntry(settings, LessonScenePath, "lesson/scene/addressables-lesson");

            var scene = EditorSceneManager.OpenScene(SampleScenePath);

            GameObject prefabObject = GameObject.Find("Addressables Examples/Prefab Spawner Example");
            GameObject spriteObject = GameObject.Find("Addressables Examples/Sprite Loader Example");
            GameObject sceneObject = GameObject.Find("Addressables Examples/Scene Loader Example");

            if (prefabObject == null || spriteObject == null || sceneObject == null)
            {
                Debug.LogError("Addressables example objects were not found in SampleScene.");
                return;
            }

            bool prefabAssigned = SetAssetReferenceGuid(prefabObject, "Learn.AddressablesExamples.AddressablesPrefabSpawner", "prefabReference", prefabEntry.guid);
            bool spriteAssigned = SetAssetReferenceGuid(spriteObject, "Learn.AddressablesExamples.AddressablesSpriteLoader", "spriteReference", spriteEntry.guid);
            bool sceneAssigned = SetAssetReferenceGuid(sceneObject, "Learn.AddressablesExamples.AddressablesSceneLoader", "sceneReference", sceneEntry.guid);

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();

            Debug.Log(
                "Addressables lesson setup complete.\n" +
                $"Prefab: {prefabPath}\n" +
                $"Sprite: {spritePath}\n" +
                $"Scene: {LessonScenePath}\n" +
                $"Assigned -> prefab:{prefabAssigned}, sprite:{spriteAssigned}, scene:{sceneAssigned}");
        }

        [MenuItem("Tools/Addressables/Build Lesson Content")]
        public static void BuildLessonContent()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            if (settings == null)
            {
                Debug.LogError("Addressables settings are not available.");
                return;
            }

            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("Addressables content build completed.");
        }

        private static AddressableAssetEntry EnsureEntry(AddressableAssetSettings settings, string path, string address, params string[] labels)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup, false, false);
            entry.address = address;

            foreach (string label in labels)
            {
                entry.SetLabel(label, true, true, false);
            }

            return entry;
        }

        private static string FindFirstSpritePath()
        {
            string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Art" });
            foreach (string guid in spriteGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadAssetAtPath<Sprite>(path) != null)
                {
                    return path;
                }
            }

            return null;
        }

        private static bool SetAssetReferenceGuid(GameObject targetObject, string componentTypeName, string propertyName, string guid)
        {
            var componentType = System.Type.GetType($"{componentTypeName}, Assembly-CSharp");
            if (componentType == null)
            {
                return false;
            }

            var component = targetObject.GetComponent(componentType);
            if (component == null)
            {
                return false;
            }

            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                return false;
            }

            SerializedProperty guidProperty = property.FindPropertyRelative("m_AssetGUID");
            if (guidProperty == null)
            {
                return false;
            }

            guidProperty.stringValue = guid;

            SerializedProperty subObjectName = property.FindPropertyRelative("m_SubObjectName");
            if (subObjectName != null)
            {
                subObjectName.stringValue = string.Empty;
            }

            SerializedProperty subObjectType = property.FindPropertyRelative("m_SubObjectType");
            if (subObjectType != null)
            {
                subObjectType.stringValue = string.Empty;
            }

            SerializedProperty editorAsset = property.FindPropertyRelative("m_EditorAsset");
            if (editorAsset != null)
            {
                editorAsset.objectReferenceValue = null;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(component);
            return true;
        }
    }
}
