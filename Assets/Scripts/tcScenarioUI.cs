
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static tcVirtualButton;

public class tcScenarioUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScenarioManager scenarioManager;
    [SerializeField] private tcVirtualButton buttonA;
    [SerializeField] private tcVirtualButton buttonB;
    [SerializeField] private TMP_Text endingText;
    [SerializeField] private tcVirtualButton restartButton;
    [SerializeField] private GameObject rightHand; //for debug only
    [SerializeField] Vector3 PressAPosition; //for debug only
    [SerializeField] Vector3 PressBPosition;
    [SerializeField] Vector3 PressRestartPosition;
    [SerializeField] Vector3 NoPressPosition;

    public event Action ChoiceA;
    public event Action ChoiceB;

    private void Start()
    {
        buttonA.scenarioUIManager = this;
        buttonB.scenarioUIManager = this;
        restartButton.scenarioUIManager = this;


        RefreshUI();
    }

    private void Update()
    {
        KeyboardDebugUpdate();

        RefreshUI();
    }

    private void KeyboardDebugUpdate()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Moving to A", this);
            rightHand.transform.position = PressAPosition;
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Moving to B", this);
            rightHand.transform.position = PressBPosition;
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Moving to C",this);
            rightHand.transform.position = PressRestartPosition;
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("Moving back", this);
            rightHand.transform.position = NoPressPosition;
        }
    }

    private void ChooseA()
    {
        Debug.Log("A choosen wisely");
        ChoiceA?.Invoke();

        RefreshUI();
    }

    private void ChooseB()
    {
        Debug.Log("B choosen wisely");
        ChoiceB?.Invoke();
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

        bool isVideoFinished =
            scenarioManager.isVideoFinished;


        buttonA.gameObject.SetActive(!isEnding && isVideoFinished);
        buttonB.gameObject.SetActive(!isEnding && isVideoFinished);






        endingText.gameObject.SetActive(isEnding);
        restartButton.gameObject.SetActive(isEnding);

        if (isEnding)
        {
            endingText.text =
                scenarioManager.CurrentNode.endingText;
            
        }
    }

    internal void ButtonPressed(tcVirtualButton.ButtonChoice choice)
    {
        if (choice == ButtonChoice.A)
            ChooseA();
        if (choice == ButtonChoice.B)
            ChooseB();
        if (choice == ButtonChoice.Restart)
            scenarioManager.RestartScenario();
    }
}
