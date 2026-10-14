


using UnityEngine;
using static UnityEngine.XR.OpenXR.Features.Interactions.HTCViveControllerProfile;

public class ScenarioManager : MonoBehaviour
{
    [Header("Scenario Data")]
    [SerializeField] private tcScenarioHolder ScenarioHolder;
    [SerializeField] private tcPanoramaController PanoramaController;
    [SerializeField] private tcVideoController VideoController;
    [SerializeField] private tcHandControler HandController;
    [SerializeField] private tcScenarioUI UIController;

    private ScenarioNodeData currentNode;
    private bool isTransitioning;
    public  bool isVideoFinished;


    public ScenarioNodeData CurrentNode => currentNode;
    public bool IsTransitioning => isTransitioning;

    private void Start()
    {
        if (ScenarioHolder == null)
        {
            Debug.LogError("ScenarioHolder не назначен.", this);
            return;
        }

        if (VideoController != null)
            VideoController.PlaybackFinished += HandleVideoFinished;

        if (!ScenarioHolder.LoadScenario())
            return;
        
        if (HandController == null)
        {
            Debug.LogError("HandController не назначен.", this);
            return;
        }

        if (UIController== null)
        {
            Debug.LogError("UIController не назначен.", this);
            return;
        }

        UIController.ChoiceA += ChooseA;
        UIController.ChoiceB += ChooseB;

        RestartScenario();
    }

    public void RestartScenario()
    {
        if (ScenarioHolder == null || !ScenarioHolder.IsLoaded)
            return;

        isTransitioning = false;
        EnterNode(ScenarioHolder.StartNodeId);
    }

    public void ChooseA()
    {
        if (currentNode != null)
            Choose(currentNode.nextA);
    }

    public void ChooseB()
    {
        if (currentNode != null)
            Choose(currentNode.nextB);
    }

    private void Choose(string nextNodeId)
    {
        if (currentNode != null &&
            currentNode.NodeKind == ScenarioNodeKind.Video &&
            !isVideoFinished)
        {
            Debug.Log("Дождитесь окончания видео.", this);
            return;
        }


        if (isTransitioning || currentNode == null)
            return;

        if (currentNode.NodeKind != ScenarioNodeKind.Video)
            return;

        if (!ScenarioHolder.TryGetNode(nextNodeId, out _))
        {
            Debug.LogError($"Не найден узел перехода: {nextNodeId}", this);
            return;
        }

        isTransitioning = true;
        EnterNode(nextNodeId);




    }

    private void EnterNode(string nodeId)
    {
        if (!ScenarioHolder.TryGetNode(nodeId, out ScenarioNodeData node))
        {
            Debug.LogError($"Не удалось открыть узел: {nodeId}", this);
            isTransitioning = false;
            return;
        }

        currentNode = node;
        isTransitioning = false;
        isVideoFinished = false;

        Debug.Log(
           $"Текущий узел: {currentNode.id}, тип: {currentNode.type}",
           this
       );

        Debug.Log($"Вы достигли нода {nodeId}");
        Debug.Log($"Choice is yours: {node.choiceAText} or {node.choiceBText}");

        if (currentNode.NodeKind == ScenarioNodeKind.Video)
        {
                VideoController.PlayVideo(currentNode.videoId);
        }

            PanoramaController.SetPanorama(currentNode.panoramaId);
            VideoController.PlayVideo(currentNode.videoId);


        if (currentNode.NodeKind == ScenarioNodeKind.Ending)
            Debug.Log(currentNode.endingText, this);
    }

    private void HandleVideoFinished()
    {
        isVideoFinished = true;
        Debug.Log("Видео завершено. Выбор A/B разрешён.", this);
    }


}

