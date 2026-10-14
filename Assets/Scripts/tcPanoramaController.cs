using System;
using UnityEngine;

public class tcPanoramaController : MonoBehaviour
{
    internal void SetPanorama(Texture2D panorama)
    {
               
        Debug.Log($"Panorama set to {panorama?.name}");
    }

    internal void SetPanorama(string panoramaFileName)
    {
        Debug.Log($"Panorama set to {panoramaFileName}");

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
