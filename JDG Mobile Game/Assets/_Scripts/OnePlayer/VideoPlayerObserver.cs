using OnePlayer.DialogueBox;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Observes a VideoPlayer component and provides functionality when the video reaches its end point.
/// </summary>
public class VideoPlayerObserver : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    /// <summary>
    /// Initializes the VideoPlayerObserver by acquiring the VideoPlayer component and setting up event listeners.
    /// </summary>
    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if(videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("No VideoPlayer component found on this GameObject!", this);
        }
    }
    
    /// <summary>
    /// Ensures that event listeners are cleaned up when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if(videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    /// <summary>
    /// Called when the video reaches its end. Triggers the corresponding dialogue event and deactivates the game object.
    /// </summary>
    /// <param name="vp">The VideoPlayer that has reached its end point.</param>
    private void OnVideoEnd(VideoPlayer vp)
    {
        DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.EndVideo);
        gameObject.SetActive(false);
    }
}