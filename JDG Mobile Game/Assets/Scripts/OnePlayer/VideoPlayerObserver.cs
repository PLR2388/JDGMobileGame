using OnePlayer.DialogueBox;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerObserver : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += CheckOver;
    }

    void CheckOver(VideoPlayer vp)
    {
        print("Video is over");
        DialogueUI.TriggerDoneEvent.Invoke(NextDialogueTrigger.EndVideo);
    }
}