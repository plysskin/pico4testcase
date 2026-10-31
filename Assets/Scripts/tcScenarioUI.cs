
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

    // Отладочная "рука" ставится на 0.2 м ближе к пользователю, чем кнопка,
    // чтобы кончик пальца дотянулся до неё (for debug only).
    private const float DebugHandOffset = 0.2f;
    private Vector3 debugHandRestPosition; //for debug only

    public event Action ChoiceA;
    public event Action ChoiceB;

    private void Start()
    {
        buttonA.scenarioUIManager = this;
        buttonB.scenarioUIManager = this;
        restartButton.scenarioUIManager = this;

        if (rightHand != null)
            debugHandRestPosition = rightHand.transform.position;

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
            MoveDebugHandTo(buttonA);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("Moving to B", this);
            MoveDebugHandTo(buttonB);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Moving to C", this);
            MoveDebugHandTo(restartButton);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("Moving back", this);
            MoveDebugHandToRest();
        }
    }

    // Панель привязана к полю зрения и постоянно смещается вместе с головой,
    // поэтому отладочная "рука" ставится относительно текущей позы кнопки:
    // на DebugHandOffset ближе к пользователю (лицом кнопка смотрит на него).
    private void MoveDebugHandTo(tcVirtualButton button)
    {
        if (rightHand == null || button == null)
            return;

        rightHand.transform.position =
            button.transform.position + button.transform.forward * DebugHandOffset;
    }

    private void MoveDebugHandToRest()
    {
        if (rightHand == null)
            return;

        rightHand.transform.position = debugHandRestPosition;
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
