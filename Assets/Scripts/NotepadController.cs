using UnityEngine;
using UnityEngine.UIElements;

public class NotepadController : MonoBehaviour
{
    [Header("UI Document Typography Settings")]
    public Font customFont;

    [Header("Puzzle Settings")]
    private const string CorrectCode = "4713";

    private Button openButton;
    private VisualElement notepadPanel;
    private TextField notepadInput;
    private PlayerSpriteScript playerMovement;

    // --- RECEPTION UI ELEMENTS ---
    private VisualElement receptionPanel;
    private TextField codeInputField;
    private Button unlockButton;
    private Button closeUiButton;

    // --- CLUE PANEL MANAGEMENT OBJECTS ---
    private VisualElement clueAPanel;
    private VisualElement clueBPanel;
    private VisualElement clueCPanel;

    private Button closeClueABtn;
    private Button closeClueBBtn;
    private Button closeClueCBtn;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // 1. LINK CORE LAYOUT OBJECTS
        openButton = root.Q<Button>("NotepadButton");
        notepadPanel = root.Q<VisualElement>("NotepadPanel");
        playerMovement = Object.FindFirstObjectByType<PlayerSpriteScript>();

        receptionPanel = root.Q<VisualElement>("ReceptionLockPanel");
        codeInputField = root.Q<TextField>("CodeInput");
        unlockButton = root.Q<Button>("SubmitCodeButton");
        closeUiButton = root.Q<Button>("CloseLockBtn");

        // 2. LINK NOTE CLUE HOOKS
        clueAPanel = root.Q<VisualElement>("ClueAPanel");
        clueBPanel = root.Q<VisualElement>("ClueBPanel");
        clueCPanel = root.Q<VisualElement>("ClueCPanel");

        closeClueABtn = root.Q<Button>("CloseClueABtn");
        closeClueBBtn = root.Q<Button>("CloseClueBBtn");
        closeClueCBtn = root.Q<Button>("CloseClueCBtn");

        // Set up the code-generated ghost text box layout for your notepad text field
        if (notepadPanel != null && notepadInput == null)
        {
            notepadInput = new TextField();
            notepadInput.multiline = true;
            notepadInput.pickingMode = PickingMode.Position;
            notepadInput.style.whiteSpace = WhiteSpace.Normal;

            notepadInput.style.backgroundColor = new StyleColor(Color.clear);
            notepadInput.style.borderTopWidth = 0f;
            notepadInput.style.borderBottomWidth = 0f;
            notepadInput.style.borderLeftWidth = 0f;
            notepadInput.style.borderRightWidth = 0f;

            var innerInputBox = notepadInput.Q(className: "unity-text-field__input");
            if (innerInputBox != null)
            {
                innerInputBox.style.backgroundColor = new StyleColor(Color.clear);
                innerInputBox.style.borderTopWidth = 0f;
                innerInputBox.style.borderBottomWidth = 0f;
                innerInputBox.style.borderLeftWidth = 0f;
                innerInputBox.style.borderRightWidth = 0f;
                innerInputBox.style.paddingLeft = 25f;
                innerInputBox.style.paddingRight = 25f;
                innerInputBox.style.paddingTop = 25f;
                innerInputBox.style.paddingBottom = 25f;
                innerInputBox.style.fontSize = 28f;

                if (customFont != null) innerInputBox.style.unityFont = new StyleFont(customFont);
                innerInputBox.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f, 1f));
            }

            notepadInput.style.width = Length.Percent(85);
            notepadInput.style.height = Length.Percent(85);
            notepadInput.style.marginLeft = Length.Auto();
            notepadInput.style.marginRight = Length.Auto();
            notepadInput.style.marginTop = Length.Auto();
            notepadInput.style.marginBottom = Length.Auto();

            notepadPanel.Add(notepadInput);
        }

        if (openButton != null) openButton.clicked += ToggleNotepad;
        if (unlockButton != null) unlockButton.clicked += CheckReceptionCode;
        if (closeUiButton != null) closeUiButton.clicked += CloseReceptionUI;

        if (closeClueABtn != null) closeClueABtn.clicked += () => CloseClueUI(clueAPanel);
        if (closeClueBBtn != null) closeClueBBtn.clicked += () => CloseClueUI(clueBPanel);
        if (closeClueCBtn != null) closeClueCBtn.clicked += () => CloseClueUI(clueCPanel);
    }

    void OnDisable()
    {
        if (openButton != null) openButton.clicked -= ToggleNotepad;
        if (unlockButton != null) unlockButton.clicked -= CheckReceptionCode;
        if (closeUiButton != null) closeUiButton.clicked -= CloseReceptionUI;
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
            notepadInput?.Focus();
            if (playerMovement != null) playerMovement.use2DMode = false;
        }
        else
        {
            notepadPanel.style.display = DisplayStyle.None;
            if (playerMovement != null) playerMovement.use2DMode = true;
        }
    }

    public void DisplayClue(string clueType)
    {
        VisualElement targetPanel = null;
        if (clueType == "ClueA") targetPanel = clueAPanel;
        if (clueType == "ClueB") targetPanel = clueBPanel;
        if (clueType == "ClueC") targetPanel = clueCPanel;

        if (targetPanel == null) return;

        targetPanel.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playerMovement != null) playerMovement.use2DMode = false;
    }

    private void CloseClueUI(VisualElement targetPanel)
    {
        if (targetPanel == null) return;
        targetPanel.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playerMovement != null) playerMovement.use2DMode = true;
    }

    public void OpenReceptionCodeScreen()
    {
        if (receptionPanel == null) return;
        receptionPanel.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        if (codeInputField != null) { codeInputField.value = ""; codeInputField.Focus(); }
        if (playerMovement != null) playerMovement.use2DMode = false;
    }

    private void CheckReceptionCode()
    {
        if (codeInputField == null || receptionPanel == null) return;
        if (codeInputField.value == CorrectCode)
        {
            Debug.Log("RECEPTION DRAWER UNLOCKED! Key to Room 2 and Room 10 added to log references.");
            CloseReceptionUI();
        }
        else
        {
            codeInputField.value = "";
            codeInputField.Focus();
        }
    }

    public void CloseReceptionUI()
    {
        if (receptionPanel == null) return;
        receptionPanel.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playerMovement != null) playerMovement.use2DMode = true;
    }
}
