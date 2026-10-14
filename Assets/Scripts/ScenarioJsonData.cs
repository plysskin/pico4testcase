
using System;

public enum ScenarioNodeKind
{
    Video,
    Ending
}

[Serializable]
public class ScenarioConfig
{
    public string startNodeId;
    public ScenarioNodeData[] nodes;
}

[Serializable]
public class ScenarioNodeData
{
    public string id;
    public string type;

    public string panoramaId;
    public string videoId;

    public string choiceAText;
    public string choiceBText;

    public string nextA;
    public string nextB;

    public string endingText;

    public ScenarioNodeKind NodeKind
    {
        get
        {
            return Enum.TryParse(
                type,
                true,
                out ScenarioNodeKind result
            )
                ? result
                : (ScenarioNodeKind)(-1);
        }
    }
}
