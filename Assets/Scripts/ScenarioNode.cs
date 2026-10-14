using UnityEngine;
using UnityEngine.Video;


public enum ScenarioNodeType
{
    Video,
    Ending
}

[CreateAssetMenu(
    fileName = "NewScenarioNode",
    menuName = "VR Scenario/Scenario Node"
)]

public class ScenarioNode : ScriptableObject
{
    [Header("Identity")]
    public string nodeId;
    public ScenarioNodeType nodeType;

    [Header("Panorama")]
    public Texture2D panorama;

    [Header("Panorama file name")]
    public string panoramaFileName;


    [Header("Video")]
    public VideoClip videoClip;

    [Header("VideoClipName")]
    public string videoClipFileName;

    [Header("Choices")]
    public string choiceAText = "A";
    public string choiceBText = "B";

    public ScenarioNode nextA;
    public ScenarioNode nextB;

    [Header("Ending")]
    [TextArea(2, 4)]
    public string endingText = "Вы достигли финала";
}
