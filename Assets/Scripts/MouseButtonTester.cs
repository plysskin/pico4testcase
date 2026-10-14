
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseButtonTester : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null || targetCamera == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = targetCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            VirtualButton button =
                hit.collider.GetComponentInParent<VirtualButton>();

            if (button != null)
                button.Press();
            else
                Debug.Log($"Попали в объект: {hit.collider.name}");
        }
        else
        {
            Debug.Log("Луч мыши ни во что не попал.");
        }
    }
}
