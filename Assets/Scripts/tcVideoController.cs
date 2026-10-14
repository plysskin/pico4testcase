using System;
using UnityEngine;
using UnityEngine.Video;

public class tcVideoController : MonoBehaviour
{

   
    [SerializeField] private VideoPlayer videoPlayer;

    public event Action PlaybackFinished;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer не назначен.", this);
            return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.source = VideoSource.VideoClip;

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public bool PlayVideo(string videoId)
    {
        if (videoPlayer == null)
            return false;

        if (string.IsNullOrWhiteSpace(videoId))
        {
            Debug.LogWarning("Не задан videoId.", this);
            return false;
        }

        VideoClip clip = Resources.Load<VideoClip>(videoId);

        if (clip == null)
        {
            Debug.LogError(
                $"Не найден видеоролик Resources/{videoId}",
                this
            );
            return false;
        }

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Play();

        Debug.Log($"Запущено видео: {videoId}", this);
        return true;
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        Debug.Log("Видео закончилось.", this);
        PlaybackFinished?.Invoke();
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}

