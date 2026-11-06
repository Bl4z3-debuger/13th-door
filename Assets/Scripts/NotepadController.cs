using UnityEngine;
using UnityEngine.UIElements;

public class NotepadController : MonoBehaviour
{
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
        notepadInput = root.Q<TextField>();

        playerMovement = Object.FindFirstObjectByType<PlayerSpriteScript>();

        if (openButton != null)
        {
            openButton.clicked += ToggleNotepad;
        }

        // --- PREVENT AUTO-SELECT ALL SETUP ---
        // This natively configures the text field so it never auto-highlights text when clicked or focused
        if (notepadInput != null)
        {
            notepadInput.textSelection.selectAllOnFocus = false;
            notepadInput.textSelection.selectAllOnMouseUp = false;
        }
        // --------------------------------------
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
