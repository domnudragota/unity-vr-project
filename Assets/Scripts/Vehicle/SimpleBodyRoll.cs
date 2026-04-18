using UnityEngine;

public class SimpleBodyRoll : MonoBehaviour
{
    [Header("References")]
    public Rigidbody carRigidbody;

    [Header("Roll Settings")]
    public float maxRollAngle = 6f;
    public float rollSpeed = 5f;
    public float speedForFullRoll = 20f;

    private Quaternion initialLocalRotation;

    private void Awake()
    {
        initialLocalRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        if (carRigidbody == null)
        {
            return;
        }

        Vector3 localVelocity = transform.parent.InverseTransformDirection(carRigidbody.linearVelocity);

        float sidewaysSpeed = localVelocity.x;
        float forwardSpeed = Mathf.Abs(localVelocity.z);

        float speedFactor = Mathf.Clamp01(forwardSpeed / speedForFullRoll);
        float targetRoll = -sidewaysSpeed * maxRollAngle * 0.05f * speedFactor;

        targetRoll = Mathf.Clamp(targetRoll, -maxRollAngle, maxRollAngle);

        Quaternion targetRotation = initialLocalRotation * Quaternion.Euler(0f, 0f, targetRoll);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            rollSpeed * Time.deltaTime
        );
    }
}