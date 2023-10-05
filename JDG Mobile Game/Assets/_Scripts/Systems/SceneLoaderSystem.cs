using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for loading different scenes.
/// </summary>
public class SceneLoaderSystem : StaticInstance<SceneLoaderSystem>
{
    // Centralized storage for scene names.
    private static class SceneNames
    {
        public const string Game = "Game";
        public const string Preload = "_preload";
        public const string MainScreen = "MainScreen";
    }

    protected override void Awake()
    {
        base.Awake();
        CheckAndLoadMainFromPreload();
    }

    /// <summary>
    /// Checks if the currently active scene is "_preload" and loads the main screen if it is.
    /// </summary>
    private void CheckAndLoadMainFromPreload()
    {
        var currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == SceneNames.Preload)
        {
            LoadMainScreen();
        }
    }

    /// <summary>
    /// Loads the main screen scene.
    /// </summary>
    public static void LoadMainScreen()
    {
        SceneManager.LoadSceneAsync(SceneNames.MainScreen, LoadSceneMode.Single);
    }

    /// <summary>
    /// Loads the game screen scene for 2 players locally.
    /// </summary>
    public static void LoadGameScreen()
    {
        SceneManager.LoadSceneAsync(SceneNames.Game, LoadSceneMode.Single);
    }
}