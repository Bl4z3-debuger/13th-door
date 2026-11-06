using UnityEngine;

public class PlayerSpriteScript : MonoBehaviour
{
    [Header("Dimension Toggle")]
    public bool use2DMode = true;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private CapsuleCollider myCollider;
    private Transform characterChild;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isCurrentlyWalking = false;

    void Start()
    {
        myCollider = GetComponent<CapsuleCollider>();

        if (transform.childCount > 0)
        {
            characterChild = transform.GetChild(0);
            spriteRenderer = characterChild.GetComponent<SpriteRenderer>();
            animator = characterChild.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // MASTER SAFETY CONTROL: If 2D Mode is turned off, skip this script!
        if (!use2DMode)
        {
            if (isCurrentlyWalking)
            {
                isCurrentlyWalking = false;
                if (animator != null)
                {
                    animator.SetBool("isWalking", false);
                    animator.Play("Walk", 0, 0f); // Resets to Frame 0
                    animator.speed = 0f;          // Freezes animation playback
                }
            }
            return;
        }

        // 1. Gather raw WASD inputs
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0, z).normalized;

        if (direction != Vector3.zero)
        {
            // Trigger animation play cycle once when movement initializes
            if (!isCurrentlyWalking)
            {
                isCurrentlyWalking = true;
                if (animator != null)
                {
                    animator.SetBool("isWalking", true);
                    animator.speed = 1f; // Resumes animation playback at normal speed
                }
            }

            // Flip the 2D graphic flatly left or right
            if (spriteRenderer != null)
            {
                if (x < 0) spriteRenderer.flipX = true;
                if (x > 0) spriteRenderer.flipX = false;
            }

            // Smooth diagonal wall sliding calculations
            Vector3 moveStepX = new Vector3(direction.x, 0, 0) * moveSpeed * Time.deltaTime;
            Vector3 moveStepZ = new Vector3(0, 0, direction.z) * moveSpeed * Time.deltaTime;

            Vector3 finalMove = Vector3.zero;

            if (!WillCollideAt(transform.position + moveStepX)) finalMove += moveStepX;
            if (!WillCollideAt(transform.position + moveStepZ)) finalMove += moveStepZ;

            transform.position += finalMove;
        }
        else
        {
            // Trigger animation pause cycle once when stationary
            if (isCurrentlyWalking)
            {
                isCurrentlyWalking = false;
                if (animator != null)
                {
                    animator.SetBool("isWalking", false);

                    // --- RESET TO FRAME 0 LOGIC ---
                    // "Walk" must match the exact name of your walk animation box inside the Animator window grid!
                    animator.Play("Walk", 0, 0f); // Overrides current state and rewinds timeline to Frame 0
                    animator.speed = 0f;          // Freezes the frame timeline right there instantly
                    // -------------------------------
                }
            }
        }
    }

    void LateUpdate()
    {
        if (use2DMode && characterChild != null && Camera.main != null)
        {
            characterChild.rotation = Camera.main.transform.rotation;
        }
    }

    bool WillCollideAt(Vector3 targetPosition)
    {
        if (myCollider == null) return false;

        float halfHeight = (myCollider.height / 2f) - myCollider.radius;
        Vector3 point0 = targetPosition + myCollider.center + Vector3.up * halfHeight;
        Vector3 point1 = targetPosition + myCollider.center - Vector3.up * halfHeight;

        Collider[] hitColliders = Physics.OverlapCapsule(point0, point1, myCollider.radius * 0.95f);
        foreach (var hit in hitColliders)
        {
            if (hit == myCollider || hit.transform.IsChildOf(transform) || hit.isTrigger)
                continue;

            return true;
        }
        return false;
    }
}
