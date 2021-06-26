using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        var currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "_preload")
        {
            SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
        }
    }
}