using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float closedAngle = 0f;
    public float openAngle = 65f;
    public float rotationSpeed = 120f;

    [Header("Input")]
    public Key toggleKey = Key.F;

    private bool isOpen = false;
    private bool keyWasPressedLastFrame = false;

    private void Update()
    {
        HandleInput();
        RotateDoor();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        bool keyPressedNow = Keyboard.current[toggleKey].isPressed;

        if (keyPressedNow && !keyWasPressedLastFrame)
        {
            isOpen = !isOpen;
        }

        keyWasPressedLastFrame = keyPressedNow;
    }

    private void RotateDoor()
    {
        float targetY = isOpen ? openAngle : closedAngle;

        Vector3 currentEuler = transform.localEulerAngles;
        float currentY = NormalizeAngle(currentEuler.y);

        float newY = Mathf.MoveTowards(currentY, targetY, rotationSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentEuler.x, newY, currentEuler.z);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}