using UnityEngine;

/// <summary>
/// Привязывает панель к полю зрения пользователя.
/// Панель — дочерний объект камеры, поэтому повторяет её поворот без задержки;
/// скрипт задаёт смещение в локальных осях камеры: вниз на <see cref="pitchDown"/>
/// градусов и вперёд на <see cref="distance"/> метров, лицевой стороной к пользователю.
/// </summary>
public class tcFovPanelAnchor : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Камера пользователя (Main Camera). Панель должна быть её дочерним объектом.")]
    [SerializeField] private Transform view;

    [Header("Placement")]
    [Tooltip("Расстояние от глаз до панели, м.")]
    [SerializeField] private float distance = 0.5f;

    [Tooltip("Насколько панель опущена ниже оси взгляда, градусы.")]
    [Range(0f, 60f)]
    [SerializeField] private float pitchDown = 32f;

    private void Awake()
    {
        if (view == null)
            view = transform.parent;

        if (view == null || transform.parent != view)
        {
            Debug.LogWarning(
                "tcFovPanelAnchor: панель не является дочерним объектом камеры — " +
                "она не будет следовать за полем зрения.",
                this);
        }

        ApplyPose();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        if (view == null)
            view = transform.parent;

        ApplyPose();
    }

    /// <summary>
    /// Пересчитывает локальную позу панели по текущим distance и pitchDown.
    /// </summary>
    public void ApplyPose()
    {
        if (view == null || transform.parent != view)
            return;

        transform.localPosition =
            Quaternion.Euler(pitchDown, 0f, 0f) * Vector3.forward * distance;

        // Разворот на 180° — лицевой стороной к пользователю.
        // Произведение кватернионов задано явно, чтобы не зависеть от порядка
        // применения осей в Quaternion.Euler(x, y, z).
        transform.localRotation =
            Quaternion.Euler(pitchDown, 0f, 0f) * Quaternion.Euler(0f, 180f, 0f);
    }
}
