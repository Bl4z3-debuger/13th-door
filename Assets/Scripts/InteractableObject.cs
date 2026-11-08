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
    [Range(0.01f, 5.0f)]
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
            instantiatedPrompt = new GameObject("Generated_E_Prompt_Canvas");
            instantiatedPrompt.transform.position = transform.position + (Vector3.up * hoverHeightOffset);

            Canvas canvas = instantiatedPrompt.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            RectTransform rect = instantiatedPrompt.GetComponent<RectTransform>();

            // --- 🌟 AUTOMATIC NATIVE RESOLUTION ASPECT RATIO OVERRIDE 🌟 ---
            // Instead of hardcoding dimensions, we read the exact pixel width/height directly from the sprite!
            // We divide by pixelsPerUnit (usually 100) to translate clean raw pixels smoothly into 3D world space metric boundaries.
            float ppu = customButtonSprite.pixelsPerUnit;
            float nativeWidth = (customButtonSprite.rect.width / ppu) * promptScaleMultiplier;
            float nativeHeight = (customButtonSprite.rect.height / ppu) * promptScaleMultiplier;

            rect.sizeDelta = new Vector2(nativeWidth, nativeHeight);
            // -----------------------------------------------------------------

            instantiatedPrompt.transform.localScale = Vector3.one;

            UnityEngine.UI.Image uiImage = instantiatedPrompt.AddComponent<UnityEngine.UI.Image>();
            uiImage.sprite = customButtonSprite;

            // Force the image layout engine to stay locked to the precise aspect ratio bounds
            uiImage.preserveAspect = true;

            instantiatedPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Dynamic position and rotation loop
        if (instantiatedPrompt != null && instantiatedPrompt.activeSelf)
        {
            instantiatedPrompt.transform.position = transform.position + (Vector3.up * hoverHeightOffset);

            if (Camera.main != null)
            {
                instantiatedPrompt.transform.rotation = Camera.main.transform.rotation;
            }

            // Keep the dimensions clean and updated relative to real-time scale changes
            RectTransform rect = instantiatedPrompt.GetComponent<RectTransform>();
            if (rect != null && customButtonSprite != null)
            {
                float ppu = customButtonSprite.pixelsPerUnit;
                rect.sizeDelta = new Vector2(
                    (customButtonSprite.rect.width / ppu) * promptScaleMultiplier,
                    (customButtonSprite.rect.height / ppu) * promptScaleMultiplier
                );
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

    void OnDestroy()
    {
        if (instantiatedPrompt != null) Destroy(instantiatedPrompt);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
