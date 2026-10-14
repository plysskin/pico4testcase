
using System.Collections.Generic;
using UnityEngine;

public class tcScenarioHolder : MonoBehaviour
{
    [Header("Scenario Source")]
    [SerializeField] private string jsonFileName = "scenario";

    private ScenarioConfig config;
    private Dictionary<string, ScenarioNodeData> nodes;

    public string StartNodeId =>
        config != null ? config.startNodeId : null;

    public int NodeCount => nodes != null ? nodes.Count : 0;

    public bool IsLoaded => nodes != null;

    public bool LoadScenario()
    {
        TextAsset jsonFile =
            Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile == null)
        {
            Debug.LogError(
                $"Не найден файл Resources/{jsonFileName}.json",
                this
            );
            return false;
        }

        ScenarioConfig loadedConfig;

        try
        {
            loadedConfig = JsonUtility.FromJson<ScenarioConfig>(
                jsonFile.text
            );
        }
        catch (System.Exception exception)
        {
            Debug.LogError(
                $"Ошибка чтения JSON: {exception.Message}",
                this
            );
            return false;
        }

        if (loadedConfig == null ||
            loadedConfig.nodes == null ||
            string.IsNullOrWhiteSpace(loadedConfig.startNodeId))
        {
            Debug.LogError("JSON сценария имеет неверную структуру.", this);
            return false;
        }

        var loadedNodes =
            new Dictionary<string, ScenarioNodeData>();

        foreach (ScenarioNodeData node in loadedConfig.nodes)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.id))
            {
                Debug.LogError("У узла отсутствует ID.", this);
                return false;
            }

            if (loadedNodes.ContainsKey(node.id))
            {
                Debug.LogError($"Повторный ID узла: {node.id}", this);
                return false;
            }

            if (node.NodeKind != ScenarioNodeKind.Video &&
                node.NodeKind != ScenarioNodeKind.Ending)
            {
                Debug.LogError(
                    $"Неизвестный тип узла {node.id}: {node.type}",
                    this
                );
                return false;
            }

            loadedNodes.Add(node.id, node);
        }

        if (!loadedNodes.ContainsKey(loadedConfig.startNodeId))
        {
            Debug.LogError(
                $"Стартовый узел не найден: {loadedConfig.startNodeId}",
                this
            );
            return false;
        }

        foreach (ScenarioNodeData node in loadedConfig.nodes)
        {
            if (node.NodeKind != ScenarioNodeKind.Video)
                continue;

            if (!loadedNodes.ContainsKey(node.nextA) ||
                !loadedNodes.ContainsKey(node.nextB))
            {
                Debug.LogError(
                    $"У узла {node.id} отсутствует корректный переход A или B.",
                    this
                );
                return false;
            }
        }

        // Заменяем рабочие данные только после успешной проверки.
        config = loadedConfig;
        nodes = loadedNodes;

        Debug.Log($"Сценарий загружен. Узлов: {NodeCount}", this);
        return true;
    }

    public bool TryGetNode(
        string nodeId,
        out ScenarioNodeData node)
    {
        node = null;

        return nodes != null &&
               !string.IsNullOrWhiteSpace(nodeId) &&
               nodes.TryGetValue(nodeId, out node);
    }
}
