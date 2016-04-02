using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;

public class ScenePrefabUtility
{
    const string PREFAB_FOLDER_PATH = "Assets/Editor/ScenePrefabs";

    [InitializeOnLoadMethod]
    static void CreatePrefabFolder()
    {
        Directory.CreateDirectory(PREFAB_FOLDER_PATH);
    }

    [PostProcessScene]
    static void OnPostProcessScene()
    {
        foreach (var sceneManager in EditorSceneManager.GetSceneManagerSetup())
        {
            var scenePath = sceneManager.path;

            var scene = SceneManager.GetSceneByPath(scenePath);

            var prefabName = string.Format("ScenePrefab({0})", scenePath);

            if (string.IsNullOrEmpty(scene.path))
                continue;

            if (GameObject.Find(prefabName) != null)
                continue;

            var prefab = GetScenePrefab(scenePath);

            if (prefab)
            {
                var go = PrefabUtility.InstantiatePrefab(prefab, scene);
                go.name = prefabName;
            }

        }
    }


    public static GameObject CreateScenePrefab(string scenePath, params System.Type[] components)
    {
        var guid = ScenePathToGUID(scenePath);
        var go = EditorUtility.CreateGameObjectWithHideFlags(guid, HideFlags.HideAndDontSave, components);

        var prefab = PrefabUtility.CreatePrefab(string.Format("{0}/{1}.prefab", PREFAB_FOLDER_PATH, guid), go);

        Object.DestroyImmediate(go);

        return prefab;
    }


    public static GameObject GetScenePrefab(string scenePath)
    {
        Directory.CreateDirectory(PREFAB_FOLDER_PATH);
        var guid = ScenePathToGUID(scenePath);
        return AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("{0}/{1}.prefab", PREFAB_FOLDER_PATH, guid));
    }

    private static string ScenePathToGUID(string scenePath)
    {
        return AssetDatabase.AssetPathToGUID(scenePath);
    }
}
