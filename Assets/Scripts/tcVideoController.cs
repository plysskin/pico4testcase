using System;
using UnityEngine;
using UnityEngine.Video;

public class tcVideoController : MonoBehaviour
{
    internal void PlayVideo(VideoClip videoClip)
    {
        Debug.Log($"videoClipPlayed {videoClip.name}");
    }

    internal void PlayVideo(string videoClipFileName)
    {
        Debug.Log($"videoClipPlayed {videoClipFileName}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
