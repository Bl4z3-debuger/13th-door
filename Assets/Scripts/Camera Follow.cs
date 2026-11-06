using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;       // Drag your parent Empty GameObject here
    public Vector3 offset = new Vector3(0f, 5f, -7f); // How high and far back the camera stays

    [Header("Movement Settings")]
    public float smoothSpeed = 10f; // Higher number = faster tracking

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculate the exact position the camera should be at
        Vector3 targetPosition = target.position + offset;

        // 2. Smoothly slide the camera toward that position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 3. Make the camera always look directly at the player
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}
