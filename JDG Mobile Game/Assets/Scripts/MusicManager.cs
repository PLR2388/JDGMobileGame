using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musics;
    // Start is called before the first frame update

    private AudioSource audioSource;
    private 
    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainScreen")
        {
            audioSource.clip = musics[0];
            if (audioSource.isPlaying == false)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}
