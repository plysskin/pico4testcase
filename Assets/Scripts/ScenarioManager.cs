


using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    [Header("Scenario Data")]
    [SerializeField] private tcScenarioHolder ScenarioHolder;
    [SerializeField] private tcPanoramaController PanoramaController;
    [SerializeField] private tcVideoController VideoController;
    [SerializeField] private tcHandControler HandController;
    private ScenarioNodeData currentNode;
    private bool isTransitioning;


    public ScenarioNodeData CurrentNode => currentNode;
    public bool IsTransitioning => isTransitioning;

    private void Start()
    {
        if (ScenarioHolder == null)
        {
            Debug.LogError("ScenarioHolder не назначен.", this);
            return;
        }

        if (!ScenarioHolder.LoadScenario())
            return;
        
        if (HandController == null)
        {
            Debug.LogError("HandController не назначен.", this);
            return;
        }


        HandController.ChoiceA += ChooseA;
        HandController.ChoiceB += ChooseB;

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

        Debug.Log(
            $"Текущий узел: {currentNode.id}, тип: {currentNode.type}",
            this
        );

            Debug.Log($"Вы достигли нода {nodeId}");
            Debug.Log($"Choice is yours: {node.choiceAText} or {node.choiceBText}");

            PanoramaController.SetPanorama(currentNode.panoramaId);
            VideoController.PlayVideo(videoClipFileName: currentNode.videoId);




        if (currentNode.NodeKind == ScenarioNodeKind.Ending)
            Debug.Log(currentNode.endingText, this);
    }
}

















///*******************************************************---  
///* СТАРЫЙ МЕНЕДЖЕР УДАЛИТЬ
///***********************************************************

//public class ScenarioManager : MonoBehaviour
//{
//    [Header("Scenario")]




//    public ScenarioNode startNode;      //TODO: make private
//    public ScenarioNode currentNode; //TODO: make private

//    private bool isTransitioning;

//    public ScenarioNode CurrentNode => currentNode;
//    public bool IsTransitioning => isTransitioning;

//    private void Start()
//    {
//        //startNode = ScenarioHolder.GetStartingScenaro();
//        Debug.Log("Hi");
//        HandController.ChoiceA += ChooseA;
//        HandController.ChoiceB += ChooseB;
//        if (ScenarioHolder == null)
//        {
//            Debug.LogError("ScenarioHolder не назначен.", this);
//            return;
//        }
//        if (!ScenarioHolder.LoadScenario())

//            return;

//        RestartScenario();
//    }

//    public void RestartScenario()
//    {
//        if (ScenarioHolder == null || !ScenarioHolder.IsLoaded)
//            return;

//        isTransitioning = false;
//        EnterNode(ScenarioHolder.StartNodeId);
//    }


//    public void ChooseA()
//    {
//        Choose(currentNode != null ? currentNode.nextA : null);
//    }

//    public void ChooseB()
//    {
//        Choose(currentNode != null ? currentNode.nextB : null);
//    }

//private void Choose(ScenarioNode nextNode)
//{
//    if (isTransitioning || currentNode == null)
//        return;

//    if (currentNode.nodeType != ScenarioNodeType.Video)
//        return;

//    if (nextNode == null)
//    {
//        Debug.LogError(
//            $"Не задан следующий узел для {currentNode.nodeId}"
//        );
//        return;
//    }

//    isTransitioning = true;
//    EnterNode(nextNode);
//}

//private void EnterNode(ScenarioNode node)
//{
//    if (node == null)
//    {
//        Debug.LogError("Стартовый или следующий узел не задан.");
//        currentNode = null;
//        isTransitioning = false;
//        return;
//    }

//    currentNode = node;
//    Debug.Log($"Вы достигли нода {node.name}");
//    Debug.Log($"Choice is yours: {node.choiceAText} or {node.choiceBText}");

//    PanoramaController.SetPanorama(currentNode.panoramaFileName);
//    VideoController.PlayVideo(videoClipFileName: currentNode.videoClipFileName);


//    isTransitioning = false;

//    Debug.Log(
//        $"Текущий узел: {node.nodeId}, тип: {node.nodeType}"
//    );
//}

///---
///







