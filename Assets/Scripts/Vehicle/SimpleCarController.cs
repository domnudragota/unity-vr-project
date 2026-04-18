using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
    [Header("Drive State")]
    public bool canDrive = true;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Visuals")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    [Header("Steering Wheel Visual")]
    public Transform steeringWheel;
    public float steeringWheelMaxRotation = 450f;

    [Header("Car Settings")]
    public float motorTorque = 900f;
    public float brakeTorque = 3000f;
    public float idleBrakeTorque = 150f;
    public float maxSteerAngle = 18f;
    public float maxSpeedKmh = 60f;

    [Header("High Speed Steering")]
    public float highSpeedSteerAngle = 7f;
    public float highSpeedKmh = 30f;
    public float steerResponseSpeed = 30f;

    [Header("Center Of Mass")]
    public Vector3 centerOfMassOffset = new Vector3(0f, -1.15f, 0f);

    private Rigidbody carRigidbody;

    private float throttleInput;
    private float steerInput;
    private bool brakeInput;

    private float currentSteerAngle;
    private Quaternion steeringWheelInitialLocalRotation;

    public float NormalizedSteerInput
    {
        get
        {
            if (maxSteerAngle <= 0.01f)
            {
                return 0f;
            }

            return Mathf.Clamp(currentSteerAngle / maxSteerAngle, -1f, 1f);
        }
    }

    private void Awake()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();

        if (carRigidbody != null)
        {
            carRigidbody.centerOfMass += centerOfMassOffset;
        }

        if (steeringWheel != null)
        {
            steeringWheelInitialLocalRotation = steeringWheel.localRotation;
        }
    }

    private void Update()
    {
        ReadInput();

        UpdateWheelVisual(frontLeftCollider, frontLeftWheel);
        UpdateWheelVisual(frontRightCollider, frontRightWheel);
        UpdateWheelVisual(rearLeftCollider, rearLeftWheel);
        UpdateWheelVisual(rearRightCollider, rearRightWheel);

        UpdateSteeringWheelVisual();
    }

    private void FixedUpdate()
    {
        ApplyMotor();
        ApplySteering();
        ApplyBrakes();
    }

    private void ReadInput()
    {
        throttleInput = 0f;
        steerInput = 0f;
        brakeInput = false;

        if (!canDrive)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            throttleInput = 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            throttleInput = -1f;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            steerInput = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steerInput = 1f;
        }

        brakeInput = Keyboard.current.spaceKey.isPressed;
    }

    private void ApplyMotor()
    {
        if (rearLeftCollider == null || rearRightCollider == null || carRigidbody == null)
        {
            return;
        }

        if (!canDrive)
        {
            rearLeftCollider.motorTorque = 0f;
            rearRightCollider.motorTorque = 0f;
            return;
        }

        float currentSpeedKmh = carRigidbody.linearVelocity.magnitude * 3.6f;
        float torque = 0f;

        if (currentSpeedKmh < maxSpeedKmh)
        {
            torque = throttleInput * motorTorque;
        }

        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    private void ApplySteering()
    {
        if (frontLeftCollider == null || frontRightCollider == null || carRigidbody == null)
        {
            return;
        }

        float currentSpeedKmh = carRigidbody.linearVelocity.magnitude * 3.6f;

        float steerAngleLimit = maxSteerAngle;

        if (currentSpeedKmh >= highSpeedKmh)
        {
            steerAngleLimit = highSpeedSteerAngle;
        }
        else
        {
            float speedLerp = currentSpeedKmh / highSpeedKmh;
            steerAngleLimit = Mathf.Lerp(maxSteerAngle, highSpeedSteerAngle, speedLerp);
        }

        if (Mathf.Abs(throttleInput) > 0.1f)
        {
            steerAngleLimit *= 0.85f;
        }

        float targetSteerAngle = canDrive ? steerInput * steerAngleLimit : 0f;

        currentSteerAngle = Mathf.MoveTowards(
            currentSteerAngle,
            targetSteerAngle,
            steerResponseSpeed * Time.fixedDeltaTime
        );

        if (Mathf.Abs(currentSteerAngle) < 0.05f)
        {
            currentSteerAngle = 0f;
        }

        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle = currentSteerAngle;
    }

    private void ApplyBrakes()
    {
        float currentBrakeTorque = 0f;

        if (brakeInput)
        {
            currentBrakeTorque = brakeTorque;
        }
        else if (canDrive && Mathf.Abs(throttleInput) < 0.01f)
        {
            currentBrakeTorque = idleBrakeTorque;
        }

        if (frontLeftCollider != null) frontLeftCollider.brakeTorque = currentBrakeTorque;
        if (frontRightCollider != null) frontRightCollider.brakeTorque = currentBrakeTorque;
        if (rearLeftCollider != null) rearLeftCollider.brakeTorque = currentBrakeTorque;
        if (rearRightCollider != null) rearRightCollider.brakeTorque = currentBrakeTorque;
    }

    private void UpdateWheelVisual(WheelCollider collider, Transform wheelTransform)
    {
        if (collider == null || wheelTransform == null)
        {
            return;
        }

        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    private void UpdateSteeringWheelVisual()
    {
        if (steeringWheel == null)
        {
            return;
        }

        float normalizedSteer = 0f;

        if (maxSteerAngle > 0.01f)
        {
            normalizedSteer = currentSteerAngle / maxSteerAngle;
        }

        float steeringWheelAngle = -normalizedSteer * steeringWheelMaxRotation;

        steeringWheel.localRotation =
            steeringWheelInitialLocalRotation *
            Quaternion.Euler(0f, 0f, steeringWheelAngle);
    }
}