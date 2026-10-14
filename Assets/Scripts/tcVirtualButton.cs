using UnityEngine;

public class tcVirtualButton : MonoBehaviour
{
    public enum ButtonChoice
    {
        A,
        B,
        Restart

    }

    [Header("Button Settings")]
    [SerializeField] private ButtonChoice choice;
    [SerializeField] public tcScenarioUI scenarioUIManager;

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

//        if (buttonRenderer != null)
//            buttonRenderer.material.color = Color.green;


        scenarioUIManager.ButtonPressed(choice);
        
        Debug.Log($"Нажата виртуальная кнопка {choice}", this);
    }

    public void Release()
    {
        isPressed = false;

        if (buttonRenderer != null)
            buttonRenderer.material.color = normalColor;
    }
}
