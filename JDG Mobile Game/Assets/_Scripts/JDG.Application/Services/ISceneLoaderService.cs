namespace JDG.Application.Services
{
    /// <summary>
    /// Service interface for scene loading operations.
    /// Phase 55: Created to replace SceneLoaderSystem singleton.
    /// </summary>
    public interface ISceneLoaderService
    {
        /// <summary>
        /// Loads the main menu screen.
        /// </summary>
        void LoadMainScreen();

        /// <summary>
        /// Loads the game screen for local 2-player mode.
        /// </summary>
        void LoadGameScreen();

        /// <summary>
        /// Loads a scene by name asynchronously.
        /// </summary>
        void LoadSceneAsync(string sceneName);

        /// <summary>
        /// Quits the game application.
        /// </summary>
        void QuitGame();
    }
}
