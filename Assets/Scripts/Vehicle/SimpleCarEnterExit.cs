using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarEnterExit : MonoBehaviour
{
    [Header("Car Control")]
    public SimpleCarController carController;

    [Header("Cameras")]
    public Camera thirdPersonCamera;
    public Camera cockpitCamera;

    [Header("Third Person Follow")]
    public SimpleFreeLookCamera thirdPersonFollowScript;
    public float reenableFollowDelay = 0.2f;

    [Header("Points")]
    public Transform entryPoint;
    public Transform exitPoint;
    public Transform lookTarget;
    public Transform headAnchor;

    [Header("Transition")]
    public float transitionDuration = 0.4f;

    [Header("Input")]
    public Key toggleKey = Key.E;

    private bool isInsideCar = false;
    private bool isTransitioning = false;
    private bool keyWasPressedLastFrame = false;

    private Coroutine reenableCoroutine;
    private Coroutine transitionCoroutine;

    private void Start()
    {
        ApplyStateImmediate();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null || isTransitioning)
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
                EnterCar();
            }
        }

        keyWasPressedLastFrame = keyPressedNow;
    }

    private void ApplyStateImmediate()
    {
        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(!isInsideCar);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(isInsideCar);
        }

        if (thirdPersonFollowScript != null)
        {
            thirdPersonFollowScript.enabled = !isInsideCar;
        }

        if (carController != null)
        {
            carController.canDrive = isInsideCar;
        }
    }

    private void EnterCar()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        if (reenableCoroutine != null)
        {
            StopCoroutine(reenableCoroutine);
            reenableCoroutine = null;
        }

        transitionCoroutine = StartCoroutine(EnterCarRoutine());
    }

    private void ExitCar()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        if (reenableCoroutine != null)
        {
            StopCoroutine(reenableCoroutine);
            reenableCoroutine = null;
        }

        transitionCoroutine = StartCoroutine(ExitCarRoutine());
    }

    private IEnumerator EnterCarRoutine()
    {
        isTransitioning = true;

        if (carController != null)
        {
            carController.canDrive = false;
        }

        if (thirdPersonFollowScript != null)
        {
            thirdPersonFollowScript.enabled = false;
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(true);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(false);
        }

        if (thirdPersonCamera != null && entryPoint != null)
        {
            thirdPersonCamera.transform.position = entryPoint.position;

            if (lookTarget != null)
            {
                Vector3 direction = lookTarget.position - thirdPersonCamera.transform.position;
                if (direction.sqrMagnitude > 0.001f)
                {
                    thirdPersonCamera.transform.rotation = Quaternion.LookRotation(direction.normalized);
                }
            }
        }

        if (thirdPersonCamera != null && headAnchor != null)
        {
            Vector3 startPosition = thirdPersonCamera.transform.position;
            Quaternion startRotation = thirdPersonCamera.transform.rotation;

            Vector3 endPosition = headAnchor.position;
            Quaternion endRotation = headAnchor.rotation;

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionDuration);
                float smoothT = t * t * (3f - 2f * t);

                thirdPersonCamera.transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
                thirdPersonCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, smoothT);

                yield return null;
            }
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(false);
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(true);
        }

        isInsideCar = true;

        if (carController != null)
        {
            carController.canDrive = true;
        }

        isTransitioning = false;
        transitionCoroutine = null;
    }

    private IEnumerator ExitCarRoutine()
    {
        isTransitioning = true;
        isInsideCar = false;

        if (carController != null)
        {
            carController.canDrive = false;
        }

        if (cockpitCamera != null)
        {
            cockpitCamera.gameObject.SetActive(false);
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.gameObject.SetActive(true);
        }

        if (thirdPersonFollowScript != null)
        {
            thirdPersonFollowScript.enabled = false;
        }

        if (thirdPersonCamera != null && headAnchor != null)
        {
            thirdPersonCamera.transform.position = headAnchor.position;
            thirdPersonCamera.transform.rotation = headAnchor.rotation;
        }

        if (thirdPersonCamera != null && exitPoint != null)
        {
            Vector3 startPosition = thirdPersonCamera.transform.position;
            Quaternion startRotation = thirdPersonCamera.transform.rotation;

            Vector3 endPosition = exitPoint.position;
            Quaternion endRotation;

            if (lookTarget != null)
            {
                Vector3 direction = lookTarget.position - exitPoint.position;
                if (direction.sqrMagnitude > 0.001f)
                {
                    endRotation = Quaternion.LookRotation(direction.normalized);
                }
                else
                {
                    endRotation = exitPoint.rotation;
                }
            }
            else
            {
                endRotation = exitPoint.rotation;
            }

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionDuration);
                float smoothT = t * t * (3f - 2f * t);

                thirdPersonCamera.transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
                thirdPersonCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, smoothT);

                yield return null;
            }
        }

        reenableCoroutine = StartCoroutine(ReenableFollowAfterDelay());

        isTransitioning = false;
        transitionCoroutine = null;
    }

    private IEnumerator ReenableFollowAfterDelay()
    {
        yield return new WaitForSeconds(reenableFollowDelay);

        if (thirdPersonFollowScript != null && !isInsideCar)
        {
            thirdPersonFollowScript.enabled = true;
        }

        reenableCoroutine = null;
    }
}