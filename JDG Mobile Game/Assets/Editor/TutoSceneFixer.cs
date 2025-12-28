using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

/// <summary>
/// Editor utility to fix the TutoPlayerGame scene by adding the missing GameSceneScope.
/// The tutorial scene needs the same DI setup as the regular Game scene to work properly.
///
/// Without GameSceneScope:
/// - VContainer doesn't inject dependencies into MonoBehaviours
/// - All Construct() methods are never called
/// - Start() methods crash with NullReferenceExceptions
///
/// This script adds the GameSceneScope component to enable proper DI.
/// </summary>
public static class TutoSceneFixer
{
    private const string GameScopeName = "GameScope";
    private const string GameSceneScopeTypeName = "JDG.DI.GameSceneScope, JDG.Legacy";
    private const string TutoSceneInitializerTypeName = "TutoSceneInitializer, JDG.Legacy";
    private const string TutoScenePath = "Assets/Scenes/TutoPlayerGame.unity";

    [MenuItem("Tools/Fix Tutorial Scene")]
    public static void FixTutoScene()
    {
        // 1. Ensure TutoPlayerGame scene is loaded
        var currentScene = EditorSceneManager.GetActiveScene();
        bool needsSceneLoad = currentScene.path != TutoScenePath;

        if (needsSceneLoad)
        {
            // Ask user if they want to save current scene first
            if (currentScene.isDirty)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Debug.Log("TutoSceneFixer: User cancelled. Scene not fixed.");
                    return;
                }
            }

            // Load the tutorial scene
            EditorSceneManager.OpenScene(TutoScenePath, OpenSceneMode.Single);
            currentScene = EditorSceneManager.GetActiveScene();
            Debug.Log($"TutoSceneFixer: Loaded scene '{currentScene.name}'");
        }

        if (!currentScene.IsValid() || currentScene.path != TutoScenePath)
        {
            Debug.LogError($"TutoSceneFixer: Failed to load TutoPlayerGame scene at '{TutoScenePath}'");
            return;
        }

        bool modified = false;

        // 2. Check if GameScope already exists
        var existingGameScope = GameObject.Find(GameScopeName);
        if (existingGameScope != null)
        {
            Debug.Log("TutoSceneFixer: GameScope already exists in scene. Checking for GameSceneScope component...");

            // Check if it has the GameSceneScope component
            var gameScopeType = Type.GetType(GameSceneScopeTypeName);
            if (gameScopeType == null)
            {
                Debug.LogError("TutoSceneFixer: Could not find GameSceneScope type. Make sure JDG.Legacy assembly is compiled.");
                return;
            }

            var existingComponent = existingGameScope.GetComponent(gameScopeType);
            if (existingComponent != null)
            {
                Debug.Log("TutoSceneFixer: GameSceneScope component already exists. Scene is already fixed!");
                return;
            }

            // GameScope exists but doesn't have GameSceneScope - add the component
            Undo.RegisterCompleteObjectUndo(existingGameScope, "Add GameSceneScope");
            var addedComponent = existingGameScope.AddComponent(gameScopeType);
            if (addedComponent != null)
            {
                Debug.Log("TutoSceneFixer: Added GameSceneScope component to existing GameScope");
                modified = true;
            }
            else
            {
                Debug.LogError("TutoSceneFixer: Failed to add GameSceneScope component");
                return;
            }
        }
        else
        {
            // 3. Create new GameScope GameObject
            Debug.Log("TutoSceneFixer: Creating new GameScope GameObject...");

            // Get the GameSceneScope type via reflection
            var gameScopeType = Type.GetType(GameSceneScopeTypeName);
            if (gameScopeType == null)
            {
                Debug.LogError($"TutoSceneFixer: Could not find type '{GameSceneScopeTypeName}'. " +
                    "Make sure the JDG.Legacy assembly is compiled.");
                return;
            }

            // Create the GameObject
            var gameScopeObj = new GameObject(GameScopeName);
            Undo.RegisterCreatedObjectUndo(gameScopeObj, "Create GameScope");

            // Add the GameSceneScope component
            var component = gameScopeObj.AddComponent(gameScopeType);
            if (component == null)
            {
                Debug.LogError("TutoSceneFixer: Failed to add GameSceneScope component to GameScope");
                Undo.DestroyObjectImmediate(gameScopeObj);
                return;
            }

            Debug.Log("TutoSceneFixer: Created GameScope with GameSceneScope component");
            modified = true;
        }

        // 4. Add TutoSceneInitializer component to GameScope (ensures tutorial decks are built)
        modified = AddTutoSceneInitializer(existingGameScope ?? GameObject.Find(GameScopeName), modified);

        // 5. Verify required components exist in scene
        VerifyRequiredComponents();

        // 5. Mark scene dirty and prompt save
        if (modified)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            Debug.Log("TutoSceneFixer: Scene marked dirty. Remember to save the scene!");
            Debug.Log("TutoSceneFixer: SUCCESS - GameSceneScope has been added to TutoPlayerGame scene.");
            Debug.Log("TutoSceneFixer: The following errors should now be fixed when you play:");
            Debug.Log("  - CardPoolManager: DeckManagementService not injected");
            Debug.Log("  - NullReferenceException in GameLoop, PlayerCards, CardLocation, etc.");
            Debug.Log("");
            Debug.Log("TutoSceneFixer: Next step: Run 'Tools > Fix Dialogue Box' to fix the dialogue text issue.");
        }
    }

    /// <summary>
    /// Adds the TutoSceneInitializer component to the GameScope GameObject.
    /// This ensures tutorial decks are built when loading the scene directly from Editor.
    /// </summary>
    /// <param name="gameScope">The GameScope GameObject to add the component to.</param>
    /// <param name="wasModified">Whether the scene was already modified.</param>
    /// <returns>True if the scene was modified.</returns>
    private static bool AddTutoSceneInitializer(GameObject gameScope, bool wasModified)
    {
        if (gameScope == null)
        {
            Debug.LogWarning("TutoSceneFixer: GameScope is null, cannot add TutoSceneInitializer");
            return wasModified;
        }

        var initializerType = Type.GetType(TutoSceneInitializerTypeName);
        if (initializerType == null)
        {
            Debug.LogWarning($"TutoSceneFixer: Could not find type '{TutoSceneInitializerTypeName}'. " +
                "This is expected if scripts haven't been compiled yet. Re-run after compilation.");
            return wasModified;
        }

        var existingInitializer = gameScope.GetComponent(initializerType);
        if (existingInitializer != null)
        {
            Debug.Log("TutoSceneFixer: TutoSceneInitializer component already exists");
            return wasModified;
        }

        Undo.RegisterCompleteObjectUndo(gameScope, "Add TutoSceneInitializer");
        var component = gameScope.AddComponent(initializerType);
        if (component != null)
        {
            Debug.Log("TutoSceneFixer: Added TutoSceneInitializer component to GameScope");
            Debug.Log("TutoSceneFixer: This ensures tutorial decks are built when loading directly from Editor");
            return true;
        }
        else
        {
            Debug.LogWarning("TutoSceneFixer: Failed to add TutoSceneInitializer component");
            return wasModified;
        }
    }

    /// <summary>
    /// Verifies that all required components exist in the scene.
    /// Logs warnings for any missing components.
    /// </summary>
    private static void VerifyRequiredComponents()
    {
        Debug.Log("TutoSceneFixer: Verifying required scene components...");

        // Components that GameSceneScope expects to find
        var requiredTypes = new[]
        {
            ("CardPoolManager", "CardPoolManager, JDG.Legacy"),
            ("UIManager", "UIManager, JDG.Legacy"),
            ("InputManager", "InputManager, JDG.Legacy"),
            ("PlayerManager", "PlayerManager, JDG.Legacy"),
        };

        foreach (var (name, typeName) in requiredTypes)
        {
            var type = Type.GetType(typeName);
            if (type == null)
            {
                Debug.LogWarning($"TutoSceneFixer: Could not find type '{typeName}'");
                continue;
            }

            var instances = UnityEngine.Object.FindObjectsByType(type, FindObjectsSortMode.None);
            if (instances.Length == 0)
            {
                Debug.LogWarning($"TutoSceneFixer: WARNING - {name} not found in scene! " +
                    "GameSceneScope will fail to register this component.");
            }
            else
            {
                Debug.Log($"TutoSceneFixer: Found {instances.Length} {name}(s)");
            }
        }

        // Check for PlayerCardManager - need 2 (player1 and player2)
        var playerCardManagerType = Type.GetType("PlayerCardManager, JDG.Legacy");
        if (playerCardManagerType != null)
        {
            var managers = UnityEngine.Object.FindObjectsByType(playerCardManagerType, FindObjectsSortMode.None);
            if (managers.Length < 2)
            {
                Debug.LogWarning($"TutoSceneFixer: WARNING - Found {managers.Length} PlayerCardManager(s), need 2!");
            }
            else
            {
                Debug.Log($"TutoSceneFixer: Found {managers.Length} PlayerCardManager(s) (need 2)");
            }
        }
    }

    [MenuItem("Tools/Fix Tutorial Scene", true)]
    private static bool ValidateFixTutoScene()
    {
        // Always enabled - we'll load the scene if needed
        return true;
    }
}
