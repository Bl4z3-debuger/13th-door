using UnityEngine;

public class TeleportTriggerPad : MonoBehaviour
{
    [Header("Destination Configuration")]
    [Tooltip("Type the exact target coordinates where the player should emerge")]
    public Vector3 targetSpawnPosition;

    private NotepadController uiController;

    void Start()
    {
        // Cache the UI overlay controller manager layout
        uiController = Object.FindFirstObjectByType<NotepadController>();
    }

    // Runs automatically when your Player Capsule touches this trigger box collider zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerSpriteScript>() != null)
        {
            if (uiController != null)
            {
                Debug.Log($"[Teleport] Player triggered room shift vector. Moving to: {targetSpawnPosition}");
                uiController.StartTeleportSequence(targetSpawnPosition);
            }
        }
    }
}
