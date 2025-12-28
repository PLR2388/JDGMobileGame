using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Editor utility to find and remove missing script references from GameObjects.
/// Use via menu: Tools > Clean Missing Scripts
/// </summary>
public static class MissingScriptCleaner
{
    [MenuItem("Tools/Clean Missing Scripts/In Current Scene")]
    public static void CleanMissingScriptsInCurrentScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("MissingScriptCleaner: No active scene");
            return;
        }

        int totalRemoved = 0;
        var rootObjects = scene.GetRootGameObjects();

        foreach (var rootObj in rootObjects)
        {
            totalRemoved += CleanMissingScriptsRecursive(rootObj);
        }

        if (totalRemoved > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"MissingScriptCleaner: Removed {totalRemoved} missing script(s) from scene '{scene.name}'. Remember to save the scene!");
        }
        else
        {
            Debug.Log($"MissingScriptCleaner: No missing scripts found in scene '{scene.name}'");
        }
    }

    [MenuItem("Tools/Clean Missing Scripts/In All Build Scenes")]
    public static void CleanMissingScriptsInAllScenes()
    {
        var scenePaths = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenePaths.Add(scene.path);
            }
        }

        if (scenePaths.Count == 0)
        {
            Debug.LogWarning("MissingScriptCleaner: No scenes in build settings");
            return;
        }

        int totalRemovedAllScenes = 0;

        foreach (var scenePath in scenePaths)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int removed = 0;

            foreach (var rootObj in scene.GetRootGameObjects())
            {
                removed += CleanMissingScriptsRecursive(rootObj);
            }

            if (removed > 0)
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"MissingScriptCleaner: Removed {removed} missing script(s) from '{scene.name}' and saved");
            }

            totalRemovedAllScenes += removed;
        }

        Debug.Log($"MissingScriptCleaner: Total removed across all scenes: {totalRemovedAllScenes}");
    }

    [MenuItem("Tools/Clean Missing Scripts/Find Missing Scripts (No Remove)")]
    public static void FindMissingScriptsInCurrentScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("MissingScriptCleaner: No active scene");
            return;
        }

        int totalMissing = 0;
        var rootObjects = scene.GetRootGameObjects();

        foreach (var rootObj in rootObjects)
        {
            totalMissing += FindMissingScriptsRecursive(rootObj);
        }

        if (totalMissing > 0)
        {
            Debug.LogWarning($"MissingScriptCleaner: Found {totalMissing} missing script(s) in scene '{scene.name}'");
        }
        else
        {
            Debug.Log($"MissingScriptCleaner: No missing scripts found in scene '{scene.name}'");
        }
    }

    private static int CleanMissingScriptsRecursive(GameObject gameObject)
    {
        int removedCount = 0;

        // Get count of missing scripts on this GameObject
        int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

        if (missingCount > 0)
        {
            Debug.Log($"MissingScriptCleaner: Removing {missingCount} missing script(s) from '{GetFullPath(gameObject)}'");

            // Remove missing scripts
            Undo.RegisterCompleteObjectUndo(gameObject, "Remove Missing Scripts");
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            removedCount += missingCount;
        }

        // Process children recursively
        foreach (Transform child in gameObject.transform)
        {
            removedCount += CleanMissingScriptsRecursive(child.gameObject);
        }

        return removedCount;
    }

    private static int FindMissingScriptsRecursive(GameObject gameObject)
    {
        int foundCount = 0;

        int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

        if (missingCount > 0)
        {
            Debug.LogWarning($"MissingScriptCleaner: Found {missingCount} missing script(s) on '{GetFullPath(gameObject)}'");
            foundCount += missingCount;
        }

        foreach (Transform child in gameObject.transform)
        {
            foundCount += FindMissingScriptsRecursive(child.gameObject);
        }

        return foundCount;
    }

    private static string GetFullPath(GameObject gameObject)
    {
        string path = gameObject.name;
        Transform parent = gameObject.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
}
