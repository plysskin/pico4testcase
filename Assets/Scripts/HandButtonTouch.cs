
using UnityEngine;
using Unity.XR.PXR;

public class HandButtonTouch : MonoBehaviour
{
    [Header("Hand Tracking")]
    [SerializeField] private PXR_Hand leftHand;
    [SerializeField] private PXR_Hand rightHand;

    [Header("Touch Settings")]
    [SerializeField] private float touchRadius = 0.04f;

    private VirtualButton leftPressedButton;
    private VirtualButton rightPressedButton;

    private void Update()
    {
        CheckHand(leftHand, ref leftPressedButton);
        CheckHand(rightHand, ref rightPressedButton);
    }

    private void CheckHand(PXR_Hand hand, ref VirtualButton pressedButton)
    {
        if (hand == null)
            return;

        //if (!hand.Computed)
        //{
        //    return;
        //}

        int tipIndex = (int)HandJoint.JointIndexTip;

        if (hand.handJoints == null ||
            hand.handJoints.Count <= tipIndex ||
            hand.handJoints[tipIndex] == null)
        {
            Debug.LogWarning($"{hand.name}: сустав кончика пальца не найден.");
            return;
        }

        Vector3 fingertip = hand.handJoints[tipIndex].position;

        VirtualButton target = FindButtonAt(fingertip);

        if (target != pressedButton)
        {
            pressedButton = target;

            if (target != null)
            {
                Debug.Log($"Палец коснулся кнопки {target.name}");
                target.Press();
            }
        }
    }


    private VirtualButton FindButtonAt(Vector3 fingertip)
    {
        Collider[] hits = Physics.OverlapSphere(fingertip, touchRadius);

        foreach (Collider hit in hits)
        {
            VirtualButton button =
                hit.GetComponentInParent<VirtualButton>();

            if (button != null)
                return button;
        }

        return null;
    }
}
