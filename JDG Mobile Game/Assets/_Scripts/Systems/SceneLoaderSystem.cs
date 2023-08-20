using UnityEngine.SceneManagement;

public class SceneLoaderSystem : PersistentSingleton<SceneLoaderSystem>
{
    public static void LoadMainScreen()
    {
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }
}
