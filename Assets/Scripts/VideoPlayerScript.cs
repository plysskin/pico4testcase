
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerTest : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer не назначен.", this);
            return;
        }

        videoPlayer.Play();
    }
}
