using UnityEngine.SceneManagement;

public class SceneLoaderSystem : StaticInstance<SceneLoaderSystem>
{
    public static void LoadMainScreen()
    {
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }
}
