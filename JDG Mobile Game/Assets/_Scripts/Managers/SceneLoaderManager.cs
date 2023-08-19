using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : PersistentSingleton<SceneLoaderManager>
{
    public static void LoadMainScreen()
    {
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }
}
