using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleHoodController : MonoBehaviour
{
    [Header("Hood Settings")]
    public float closedAngle = 0f;
    public float openAngle = -55f;
    public float rotationSpeed = 100f;

    [Header("Input")]
    public Key toggleKey = Key.H;

    private bool isOpen = false;
    private bool keyWasPressedLastFrame = false;

    private void Update()
    {
        HandleInput();
        RotateHood();
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

    private void RotateHood()
    {
        float targetX = isOpen ? openAngle : closedAngle;

        Vector3 currentEuler = transform.localEulerAngles;
        float currentX = NormalizeAngle(currentEuler.x);

        float newX = Mathf.MoveTowards(currentX, targetX, rotationSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(newX, currentEuler.y, currentEuler.z);
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