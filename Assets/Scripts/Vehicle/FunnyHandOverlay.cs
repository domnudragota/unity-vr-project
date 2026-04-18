using UnityEngine;

public class FunnyHandOverlay : MonoBehaviour
{
    [Header("References")]
    public SimpleCarController carController;
    public SimpleCarEnterExit enterExitSystem;
    public RectTransform handImageRect;

    [Header("Rotation")]
    public float maxHandRotation = 18f;
    public float rotationSmoothSpeed = 8f;

    private Quaternion initialRotation;

    private void Awake()
    {
        if (handImageRect != null)
        {
            initialRotation = handImageRect.localRotation;
        }
    }

    private void Update()
    {
        UpdateVisibility();
        UpdateRotation();
    }

    private void UpdateVisibility()
    {
        if (handImageRect == null || enterExitSystem == null)
        {
            return;
        }

        bool shouldShow = enterExitSystem.IsInsideCar;
        if (handImageRect.gameObject.activeSelf != shouldShow)
        {
            handImageRect.gameObject.SetActive(shouldShow);
        }
    }

    private void UpdateRotation()
    {
        if (handImageRect == null || carController == null)
        {
            return;
        }

        float steer = carController.NormalizedSteerInput;
        float targetAngle = -steer * maxHandRotation;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 0f, targetAngle);

        handImageRect.localRotation = Quaternion.Slerp(
            handImageRect.localRotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}