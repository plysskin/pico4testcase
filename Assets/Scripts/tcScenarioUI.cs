
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class tcScenarioUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScenarioManager scenarioManager;
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private Button restartButton;
    public event Action ChoiceA;
    public event Action ChoiceB;

    private void Start()
    {
        buttonA.onClick.AddListener(ChooseA);
        buttonB.onClick.AddListener(ChooseB);
        restartButton.onClick.AddListener(RestartScenario);

        RefreshUI();
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("A choosen wisely");
            ChoiceA?.Invoke();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("B choosen wisely");
            ChoiceB?.Invoke();
        }

        RefreshUI();
    }

    private void ChooseA()
    {
        ChoiceA?.Invoke();

//        scenarioManager.ChooseA();
        RefreshUI();
    }

    private void ChooseB()
    {
        ChoiceB?.Invoke();
//        scenarioManager.ChooseB();
        RefreshUI();
    }

    private void RestartScenario()
    {
        scenarioManager.RestartScenario();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (scenarioManager == null ||
            scenarioManager.CurrentNode == null)
            return;

        bool isEnding =
            scenarioManager.CurrentNode.NodeKind ==
            ScenarioNodeKind.Ending;

        buttonA.gameObject.SetActive(!isEnding);
        buttonB.gameObject.SetActive(!isEnding);

        endingText.gameObject.SetActive(isEnding);
        restartButton.gameObject.SetActive(isEnding);

        if (isEnding)
        {
            endingText.text =
                scenarioManager.CurrentNode.endingText;
        }
    }



}
