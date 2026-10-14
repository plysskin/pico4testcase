using System;
using UnityEngine;



public class tcPanoramaController : MonoBehaviour
{
    [SerializeField] private Material skyboxTemplate;


    internal void SetPanorama(string panoramaFileName)
    {
        Debug.Log($"Panorama set to {panoramaFileName}");
        LoadPanorama(panoramaFileName);
    }



    private Material runtimeSkybox;

    private void Awake()
    {
        if (skyboxTemplate == null)
        {
            Debug.LogError("Не назначен Skybox Template.", this);
            return;
        }

        runtimeSkybox = new Material(skyboxTemplate);
    }

    public bool LoadPanorama(string panoramaId)
    {
        if (runtimeSkybox == null)
            return false;

        if (string.IsNullOrWhiteSpace(panoramaId))
            return false;

        Texture texture = Resources.Load<Texture>(panoramaId);

        if (texture == null)
        {
            Debug.LogError(
                $"Не найдена панорама Resources/{panoramaId}",
                this
            );
            return false;
        }

        runtimeSkybox.SetTexture("_MainTex", texture);
        RenderSettings.skybox = runtimeSkybox;

        DynamicGI.UpdateEnvironment();

        Debug.Log($"Загружена панорама: {panoramaId}", this);
        return true;
    }
}
