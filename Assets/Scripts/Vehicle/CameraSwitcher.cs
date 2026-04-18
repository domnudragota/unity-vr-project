using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    public Camera thirdPersonCamera;
    public Camera cockpitCamera;

    [Header("Input")]
    public Key toggleKey = Key.C;

    private bool isCockpitActive = false;
    private bool keyWasPressedLastFrame = false;

    private void Start()
    {
        ApplyCameraState();
    }

    private void Update()
    {
        HandleInput();
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
            isCockpitActive = !isCockpitActive;
            ApplyCameraState();
        }

        keyWasPressedLastFrame = keyPressedNow;
    }

    private void ApplyCameraState()
    {
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(!isCockpitActive);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(isCockpitActive);
        }
    }
}