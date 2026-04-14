using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
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

    [Header("Car Settings")]
    public float motorTorque = 2500f;
    public float brakeTorque = 2000f;
    public float maxSteerAngle = 30f;

    [Header("Center Of Mass")]
    public Vector3 centerOfMassOffset = new Vector3(0f, -0.5f, 0f);

    private Rigidbody carRigidbody;

    private float throttleInput;
    private float steerInput;
    private bool brakeInput;

    private void Awake()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();

        if (carRigidbody != null)
        {
            carRigidbody.centerOfMass += centerOfMassOffset;
        }
    }

    private void Update()
    {
        ReadInput();
        UpdateWheelVisual(frontLeftCollider, frontLeftWheel);
        UpdateWheelVisual(frontRightCollider, frontRightWheel);
        UpdateWheelVisual(rearLeftCollider, rearLeftWheel);
        UpdateWheelVisual(rearRightCollider, rearRightWheel);
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
        if (rearLeftCollider == null || rearRightCollider == null)
        {
            return;
        }

        float torque = throttleInput * motorTorque;

        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    private void ApplySteering()
    {
        if (frontLeftCollider == null || frontRightCollider == null)
        {
            return;
        }

        float steerAngle = steerInput * maxSteerAngle;

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    private void ApplyBrakes()
    {
        float currentBrakeTorque = brakeInput ? brakeTorque : 0f;

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
}