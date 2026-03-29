using JDG.Application.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ISceneLoaderService.
    /// Phase 55: Created to replace SceneLoaderSystem singleton.
    /// </summary>
    public class SceneLoaderService : ISceneLoaderService
    {
        // Centralized storage for scene names.
        private static class SceneNames
        {
            public const string Game = "Game";
            public const string MainScreen = "MainScreen";
            public const string TutorialScene = "TutoPlayerGame";
        }

        /// <summary>
        /// Loads the main menu screen.
        /// </summary>
        public void LoadMainScreen()
        {
            SceneManager.LoadSceneAsync(SceneNames.MainScreen, LoadSceneMode.Single);
        }

        /// <summary>
        /// Loads the game screen for local 2-player mode.
        /// </summary>
        public void LoadGameScreen()
        {
            SceneManager.LoadSceneAsync(SceneNames.Game, LoadSceneMode.Single);
        }

        /// <summary>
        /// Loads a scene by name asynchronously.
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        /// <summary>
        /// Quits the game application.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("SceneLoaderService: Quitting game...");
#endif
            UnityEngine.Application.Quit();
        }
    }
}
