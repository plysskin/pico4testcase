using UnityEngine;

public class VirtualButton : MonoBehaviour
{
    public enum ButtonChoice
    {
        A,
        B
    }

    [Header("Button Settings")]
    [SerializeField] private ButtonChoice choice;
    [SerializeField] private ScenarioManager scenarioManager;

    private Renderer buttonRenderer;
    private Color normalColor;
    private bool isPressed;

    private void Awake()
    {
        buttonRenderer = GetComponent<Renderer>();

        if (buttonRenderer != null)
            normalColor = buttonRenderer.material.color;
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown сработал", this);
        Press();
    }

    public void Press()
    {
        if (isPressed || scenarioManager == null)
            return;

        isPressed = true;

        if (buttonRenderer != null)
            buttonRenderer.material.color = Color.green;

        if (choice == ButtonChoice.A)
            scenarioManager.ChooseA();
        else
            scenarioManager.ChooseB();

        Debug.Log($"Нажата виртуальная кнопка {choice}", this);
    }

    public void Release()
    {
        isPressed = false;

        if (buttonRenderer != null)
            buttonRenderer.material.color = normalColor;
    }
}
