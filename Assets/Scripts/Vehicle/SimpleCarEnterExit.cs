using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarEnterExit : MonoBehaviour
{
    [Header("Car Control")]
    public SimpleCarController carController;

    [Header("XR")]
    public GameObject xrRigRoot;
    public Transform xrCamera;

    [Header("Car Camera")]
    public Camera cockpitCamera;

    [Header("Points")]
    public Transform entryPoint;
    public Transform headAnchor;
    public Transform exitPoint;

    [Header("Interaction")]
    public Key toggleKey = Key.E;
    public float enterDistance = 2.0f;

    private bool isInsideCar = false;
    private bool keyWasPressedLastFrame = false;

    public bool IsInsideCar => isInsideCar;

    private void Start()
    {
        ApplyStateImmediate();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        bool keyPressedNow = Keyboard.current[toggleKey].isPressed;

        if (keyPressedNow && !keyWasPressedLastFrame)
        {
            if (isInsideCar)
            {
                ExitCar();
            }
            else
            {
                TryEnterCar();
            }
        }

        keyWasPressedLastFrame = keyPressedNow;
    }

    private void TryEnterCar()
    {
        if (xrCamera == null || entryPoint == null)
        {
            return;
        }

        float distance = Vector3.Distance(xrCamera.position, entryPoint.position);

        if (distance > enterDistance)
        {
            return;
        }

        EnterCar();
    }

    private void EnterCar()
    {
        isInsideCar = true;

        if (carController != null)
        {
            carController.canDrive = true;
        }

        if (xrRigRoot != null)
        {
            xrRigRoot.SetActive(false);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(true);
        }
    }

    private void ExitCar()
    {
        isInsideCar = false;

        if (carController != null)
        {
            carController.canDrive = false;
        }

        if (xrRigRoot != null)
        {
            xrRigRoot.SetActive(true);
        }

        if (xrRigRoot != null && xrCamera != null && exitPoint != null)
        {
            Transform xrRigTransform = xrRigRoot.transform;

            Vector3 cameraOffset = xrCamera.position - xrRigTransform.position;
            xrRigTransform.position = exitPoint.position - cameraOffset;

            Vector3 currentEuler = xrRigTransform.eulerAngles;
            xrRigTransform.rotation = Quaternion.Euler(currentEuler.x, exitPoint.eulerAngles.y, currentEuler.z);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(false);
        }
    }

    private void ApplyStateImmediate()
    {
        if (carController != null)
        {
            carController.canDrive = isInsideCar;
        }

        if (xrRigRoot != null)
        {
            xrRigRoot.SetActive(!isInsideCar);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(isInsideCar);
        }
    }
}