using UnityEngine;
using UnityEngine.SceneManagement;

namespace JDG.Infrastructure.Bootstrap
{
    /// <summary>
    /// Initializes the game from the _preload scene by automatically loading MainScreen.
    /// Phase 84 Fix: Replaces deleted SceneLoaderSystem auto-load behavior.
    /// </summary>
    public class PreloadInitializer : MonoBehaviour
    {
        private const string PreloadSceneName = "_preload";
        private const string MainScreenSceneName = "MainScreen";

        private void Start()
        {
            var currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == PreloadSceneName)
            {
#if UNITY_EDITOR
                Debug.Log("PreloadInitializer: Starting from _preload, loading MainScreen...");
#endif
                SceneManager.LoadSceneAsync(MainScreenSceneName, LoadSceneMode.Single);
            }
        }
    }
}
