using UnityEngine;

public enum InteractionType { ReceptionDesk, ClueA_Note, ClueB_GuestBook, ClueC_Calendar }

public class InteractableObject : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public InteractionType objectType;
    public float interactionRadius = 5.0f;

    [Header("Position Offsets")]
    [Tooltip("How high above the object the E icon should float")]
    public float hoverHeightOffset = 2.0f;

    [Header("Scale Adjustments")]
    [Tooltip("Adjust this to scale the prompt image size up or down smoothly!")]
    [Range(0.1f, 5.0f)]
    public float promptScaleMultiplier = 1.0f;

    [Header("Visual Custom Asset Assignment")]
    [Tooltip("Drag and drop your custom 'makebead-128x66_1' Sprite asset right here!")]
    public Sprite customButtonSprite;

    private Transform playerTransform;
    private bool playerIsClose = false;
    private NotepadController uiController;

    // Internal code-generated hover objects
    private GameObject instantiatedPrompt;

    void Start()
    {
        var playerScript = Object.FindFirstObjectByType<PlayerSpriteScript>();
        if (playerScript != null) playerTransform = playerScript.transform;
        uiController = Object.FindFirstObjectByType<NotepadController>();

        if (customButtonSprite != null)
        {
            // --- 🌟 UN-WARPED INDEPENDENT CONTAINER ENGINE 🌟 ---
            // We create the prompt completely independent of the 3D object hierarchies.
            // This prevents it from inheriting distorted scale data from parent meshes!
            instantiatedPrompt = new GameObject("Generated_E_Prompt_Canvas");
            instantiatedPrompt.transform.position = transform.position + (Vector3.up * hoverHeightOffset);

            Canvas canvas = instantiatedPrompt.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            RectTransform rect = instantiatedPrompt.GetComponent<RectTransform>();

            // Standardizing a clean aspect ratio multiplier scale in absolute coordinates
            float baseWidth = 1.28f * promptScaleMultiplier;
            float baseHeight = 0.66f * promptScaleMultiplier;
            rect.sizeDelta = new Vector2(baseWidth, baseHeight);

            // Lock scale firmly to 1, completely immune to parent object sizes!
            instantiatedPrompt.transform.localScale = Vector3.one;

            UnityEngine.UI.Image uiImage = instantiatedPrompt.AddComponent<UnityEngine.UI.Image>();
            uiImage.sprite = customButtonSprite;

            instantiatedPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Dynamic position tracking loop (keeps the prompt floating right over moving or stationary targets)
        if (instantiatedPrompt != null && instantiatedPrompt.activeSelf)
        {
            instantiatedPrompt.transform.position = transform.position + (Vector3.up * hoverHeightOffset);

            // Face camera lens vectors completely flat
            if (Camera.main != null)
            {
                instantiatedPrompt.transform.rotation = Camera.main.transform.rotation;
            }

            // Real-time scale fine-tuning multiplier override
            RectTransform rect = instantiatedPrompt.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(1.28f * promptScaleMultiplier, 0.66f * promptScaleMultiplier);
            }
        }

        if (playerTransform == null || uiController == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionRadius)
        {
            if (!playerIsClose)
            {
                playerIsClose = true;
                if (instantiatedPrompt != null) instantiatedPrompt.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TriggerInteraction();
            }
        }
        else
        {
            if (playerIsClose)
            {
                playerIsClose = false;
                if (instantiatedPrompt != null) instantiatedPrompt.SetActive(false);
            }
        }
    }

    private void TriggerInteraction()
    {
        if (instantiatedPrompt != null) instantiatedPrompt.SetActive(false);

        switch (objectType)
        {
            case InteractionType.ReceptionDesk:
                uiController.OpenReceptionCodeScreen();
                break;
            case InteractionType.ClueA_Note:
                uiController.DisplayClue("ClueA");
                break;
            case InteractionType.ClueB_GuestBook:
                uiController.DisplayClue("ClueB");
                break;
            case InteractionType.ClueC_Calendar:
                uiController.DisplayClue("ClueC");
                break;
        }
    }

    // Cleans up un-parented runtime elements if the main object is destroyed
    void OnDestroy()
    {
        if (instantiatedPrompt != null)
        {
            Destroy(instantiatedPrompt);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
