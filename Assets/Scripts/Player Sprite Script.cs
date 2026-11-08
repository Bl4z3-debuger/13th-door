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
        if (!use2DMode)
        {
            if (isCurrentlyWalking)
            {
                isCurrentlyWalking = false;
                // --- 🌟 FORCE ANIMATION TO FREEZE WHEN INPUT IS LOCKED 🌟 ---
                if (animator != null)
                {
                    animator.speed = 0f;
                }
            }
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, 0, z).normalized;

        if (direction != Vector3.zero)
        {
            if (!isCurrentlyWalking)
            {
                isCurrentlyWalking = true;
                // --- 🌟 WAKE ANIMATION SPEED BACK UP 🌟 ---
                if (animator != null)
                {
                    animator.speed = 1f;
                    if (HasParameter("isWalking", animator)) animator.SetBool("isWalking", true);
                }
            }

            if (spriteRenderer != null)
            {
                if (x < 0) spriteRenderer.flipX = true;
                if (x > 0) spriteRenderer.flipX = false;
            }

            Vector3 moveStepX = new Vector3(direction.x, 0, 0) * moveSpeed * Time.deltaTime;
            Vector3 moveStepZ = new Vector3(0, 0, direction.z) * moveSpeed * Time.deltaTime;

            Vector3 finalMove = Vector3.zero;

            if (!WillCollideAt(transform.position + moveStepX)) finalMove += moveStepX;
            if (!WillCollideAt(transform.position + moveStepZ)) finalMove += moveStepZ;

            transform.position += finalMove;
        }
        else
        {
            if (isCurrentlyWalking)
            {
                isCurrentlyWalking = false;
                // --- 🌟 FORCE ANIMATION TO FREEZE WHEN STANDING STILL 🌟 ---
                if (animator != null)
                {
                    animator.speed = 0f;
                    if (HasParameter("isWalking", animator)) animator.SetBool("isWalking", false);
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

    // Dynamic verification helper to check if a parameter exists before using it
    private bool HasParameter(string paramName, Animator anim)
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}
