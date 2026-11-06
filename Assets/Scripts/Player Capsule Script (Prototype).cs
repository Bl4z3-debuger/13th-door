using UnityEngine;
using UnityEngine.UIElements;

public class Player_script : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CapsuleCollider myCollider;

    void Start()
    {
        // Automatically get the Capsule Collider you added to the parent
        myCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // 1. Get WASD keys
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0, z).normalized;

        if (direction != Vector3.zero)
        {
            // 2. Turn to look where you want to go
            transform.forward = direction;

            // 3. Calculate where the player WANTS to move next frame
            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // 4. Check if that next position will crash into a wall
            if (!WillCollideAt(nextPosition))
            {
                // Path is clear! Move safely
                transform.position = nextPosition;
            }
        }

        // 5. Detect W key press
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W Key was pressed!");
        }
    }

    // Helper method that projects the capsule collider forward to test for walls
    bool WillCollideAt(Vector3 targetPosition)
    {
        // Calculate the top and bottom spheres of your capsule collider
        float halfHeight = (myCollider.height / 2f) - myCollider.radius;
        Vector3 point0 = targetPosition + myCollider.center + Vector3.up * halfHeight;
        Vector3 point1 = targetPosition + myCollider.center - Vector3.up * halfHeight;

        // Scan the area at the target position. 
        // We use a slightly smaller radius (95%) so it doesn't get stuck on the floor.
        Collider[] hitColliders = Physics.OverlapCapsule(point0, point1, myCollider.radius * 0.95f);

        foreach (var hit in hitColliders)
        {
            // Ignore collisions with yourself
            if (hit == myCollider || hit.transform.IsChildOf(transform))
                continue;

            // Ignore triggers (like coins or checkpoints)
            if (hit.isTrigger)
                continue;

            // If it hits anything else (like a wall), block movement!
            return true;
        }

        return false;
    }
}
