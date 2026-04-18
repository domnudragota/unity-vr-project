using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleFreeLookCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance")]
    public float distance = 4.2f;
    public float height = 1.6f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.5f;
    public float minPitch = -10f;
    public float maxPitch = 35f;

    [Header("Follow")]
    public float followSmoothSpeed = 10f;
    public float rotationSmoothSpeed = 12f;

    private float yaw;
    private float pitch = 10f;

    private void Start()
    {
        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        ReadMouseInput();
        UpdateCameraPosition();
    }

    private void ReadMouseInput()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime * 50f;
        pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime * 50f;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void UpdateCameraPosition()
    {
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetOffset = targetRotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = target.position + Vector3.up * height + targetOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothSpeed * Time.deltaTime
        );

        Quaternion desiredRotation = Quaternion.LookRotation((target.position + Vector3.up * height * 0.5f) - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}