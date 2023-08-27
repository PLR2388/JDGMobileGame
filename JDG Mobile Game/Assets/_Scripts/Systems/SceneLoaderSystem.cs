using UnityEngine.SceneManagement;

public class SceneLoaderSystem : StaticInstance<SceneLoaderSystem>
{

    protected override void Awake()
    {
        base.Awake();
        var currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "_preload")
        {
            LoadMainScreen();
        }
    }
    public static void LoadMainScreen()
    {
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }
}
