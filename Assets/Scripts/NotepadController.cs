using UnityEngine;
using UnityEngine.UIElements;

public class NotepadController : MonoBehaviour
{
    [Header("UI Document Typography Settings")]
    [Tooltip("Drag and drop your 'IndieFlower-Regular' font file asset here from your project folders")]
    public Font customFont;

    private Button openButton;
    private VisualElement notepadPanel;
    private TextField notepadInput;
    private PlayerSpriteScript playerMovement;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        openButton = root.Q<Button>("NotepadButton");
        notepadPanel = root.Q<VisualElement>("NotepadPanel");
        playerMovement = Object.FindFirstObjectByType<PlayerSpriteScript>();

        // --- DIRECT RUNTIME COMPONENT INJECTION ---
        if (notepadPanel != null && notepadInput == null)
        {
            // 1. Create a fresh text field via code
            notepadInput = new TextField();
            notepadInput.pickingMode = PickingMode.Position;

            // --- MULTILINE & WORD WRAPPING FORMATTING ---
            notepadInput.multiline = true;
            notepadInput.style.whiteSpace = WhiteSpace.Normal;
            // ---------------------------------------------

            // 2. Clear outer boundaries
            notepadInput.style.backgroundColor = new StyleColor(Color.clear);
            notepadInput.style.borderTopWidth = 0f;
            notepadInput.style.borderBottomWidth = 0f;
            notepadInput.style.borderLeftWidth = 0f;
            notepadInput.style.borderRightWidth = 0f;

            // 3. Force the internal unity-text-input block to be transparent and apply font formatting
            var innerInputBox = notepadInput.Q(className: "unity-text-field__input");
            if (innerInputBox != null)
            {
                innerInputBox.style.backgroundColor = new StyleColor(Color.clear);
                innerInputBox.style.borderTopWidth = 0f;
                innerInputBox.style.borderBottomWidth = 0f;
                innerInputBox.style.borderLeftWidth = 0f;
                innerInputBox.style.borderRightWidth = 0f;

                // Generous text margins so letters don't hug your pixel-art borders
                innerInputBox.style.paddingLeft = 25f;
                innerInputBox.style.paddingRight = 25f;
                innerInputBox.style.paddingTop = 25f;
                innerInputBox.style.paddingBottom = 25f;

                // Font scale size
                innerInputBox.style.fontSize = 28f;

                // Drag-and-drop font asset application
                if (customFont != null)
                {
                    innerInputBox.style.unityFont = new StyleFont(customFont);
                }

                // Ink color
                innerInputBox.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f, 1f));
            }

            // 4. Center it perfectly inside your paper asset margins
            notepadInput.style.width = Length.Percent(85);
            notepadInput.style.height = Length.Percent(85);
            notepadInput.style.marginLeft = Length.Auto();
            notepadInput.style.marginRight = Length.Auto();
            notepadInput.style.marginTop = Length.Auto();
            notepadInput.style.marginBottom = Length.Auto();

            // 5. Inject the clean ghost field inside your notebook panel visual
            notepadPanel.Add(notepadInput);
        }

        if (openButton != null)
        {
            openButton.clicked += ToggleNotepad;
        }

        // Keep background panel click-back focus working smoothly
        if (notepadPanel != null)
        {
            notepadPanel.RegisterCallback<PointerDownEvent>(evt => {
                if (notepadPanel.style.display == DisplayStyle.Flex && notepadInput != null)
                {
                    notepadInput.Focus();
                }
            });
        }
    }

    void OnDisable()
    {
        if (openButton != null)
        {
            openButton.clicked -= ToggleNotepad;
        }
    }

    private void ToggleNotepad()
    {
        if (notepadPanel == null) return;

        bool isOpening = notepadPanel.style.display != DisplayStyle.Flex;

        if (isOpening)
        {
            notepadPanel.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            if (notepadInput != null)
            {
                notepadInput.Focus();
                notepadInput.SelectNone();
            }

            if (playerMovement != null) playerMovement.use2DMode = false;
        }
        else
        {
            notepadPanel.style.display = DisplayStyle.None;
            if (playerMovement != null) playerMovement.use2DMode = true;
        }
    }
}
