using UnityEngine;
using System.Collections;
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

    // --- CLUE PANEL HOOKS ---
    private VisualElement clueAPanel;
    private VisualElement clueBPanel;
    private VisualElement clueCPanel;
    private Button closeClueABtn;
    private Button closeClueBBtn;
    private Button closeClueCBtn;

    // --- SCREEN FADE MANAGEMENT HOOKS ---
    private VisualElement fadePanel;
    private bool isFading = false;

    // --- DIALOGUE SYSTEM HOOKS ---
    private VisualElement dialoguePanel;
    private Label dialogueTextLabel;
    private bool isDialogueActive = false;

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

        // 3. LINK SCREEN FADE & DIALOGUE HOOKS
        fadePanel = root.Q<VisualElement>("ScreenFadePanel");
        dialoguePanel = root.Q<VisualElement>("DialoguePanel");
        dialogueTextLabel = root.Q<Label>("DialogueText");

        // Set up the code-generated ghost text box layout for your notepad text field
        if (notepadPanel != null && notepadInput == null)
        {
            notepadInput = new TextField();
            notepadInput.multiline = true;
            notepadInput.pickingMode = PickingMode.Position;
            notepadPanel.pickingMode = PickingMode.Position;

            notepadInput.style.whiteSpace = WhiteSpace.Normal;
            notepadInput.style.backgroundColor = new StyleColor(Color.clear);
            notepadInput.style.borderTopWidth = 0f;
            notepadInput.style.borderBottomWidth = 0f;
            notepadInput.style.borderLeftWidth = 0f;
            notepadInput.style.borderRightWidth = 0f;

            var innerInputBox = notepadInput.Q(className: "unity-text-field__input");
            if (innerInputBox != null)
            {
                innerInputBox.pickingMode = PickingMode.Position;
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

        // 4. ATTACH THE EVENT CLICK LISTENERS
        if (openButton != null) openButton.clicked += ToggleNotepad;
        if (unlockButton != null) unlockButton.clicked += CheckReceptionCode;
        if (closeUiButton != null) closeUiButton.clicked += CloseReceptionUI;

        if (closeClueABtn != null) closeClueABtn.clicked += CloseClueA;
        if (closeClueBBtn != null) closeClueBBtn.clicked += CloseClueB;
        if (closeClueCBtn != null) closeClueCBtn.clicked += CloseClueC;

        if (notepadPanel != null)
        {
            notepadPanel.RegisterCallback<PointerDownEvent>(FocusNotepadInput);
        }
    }

    void OnDisable()
    {
        if (openButton != null) openButton.clicked -= ToggleNotepad;
        if (unlockButton != null) unlockButton.clicked -= CheckReceptionCode;
        if (closeUiButton != null) closeUiButton.clicked -= CloseReceptionUI;

        if (closeClueABtn != null) closeClueABtn.clicked -= CloseClueA;
        if (closeClueBBtn != null) closeClueBBtn.clicked -= CloseClueB;
        if (closeClueCBtn != null) closeClueCBtn.clicked -= CloseClueC;

        if (notepadPanel != null)
        {
            notepadPanel.UnregisterCallback<PointerDownEvent>(FocusNotepadInput);
        }
    }

    private void FocusNotepadInput(PointerDownEvent evt)
    {
        if (notepadPanel.style.display == DisplayStyle.Flex && notepadInput != null)
        {
            notepadInput.Focus();
            evt.StopPropagation();
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
            notepadInput?.Focus();
            if (playerMovement != null) playerMovement.use2DMode = false;
        }
        else
        {
            notepadPanel.style.display = DisplayStyle.None;
            if (playerMovement != null) playerMovement.use2DMode = true;
        }
    }

    // --- SCREEN FADE TELEPORT LOOP ---
    public void StartTeleportSequence(Vector3 destination)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndTeleportRoutine(destination));
        }
    }

    private IEnumerator FadeAndTeleportRoutine(Vector3 destination)
    {
        isFading = true;

        if (playerMovement != null) playerMovement.use2DMode = false;

        if (fadePanel != null)
        {
            fadePanel.style.backgroundColor = new StyleColor(Color.black);
        }

        yield return new WaitForSeconds(0.5f);

        if (playerMovement != null)
        {
            playerMovement.transform.position = destination;
        }

        yield return new WaitForSeconds(0.1f);

        if (fadePanel != null)
        {
            fadePanel.style.backgroundColor = new StyleColor(Color.clear);
        }

        yield return new WaitForSeconds(0.5f);

        if (playerMovement != null) playerMovement.use2DMode = true;
        isFading = false;
    }

    // --- DYNAMIC CLUE INTERACT FUNCTIONS ---
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

    private void CloseClueA() => CloseClueUI(clueAPanel);
    private void CloseClueB() => CloseClueUI(clueBPanel);
    private void CloseClueC() => CloseClueUI(clueCPanel);

    private void CloseClueUI(VisualElement targetPanel)
    {
        if (targetPanel == null) return;
        targetPanel.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (playerMovement != null) playerMovement.use2DMode = true;
    }

    // --- RECEPTION CODE METHODS ---
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
            Debug.Log("RECEPTION DRAWER UNLOCKED! Rewards injected into logs.");
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

    // --- DIALOGUE ENGINE TRACKING LOOP ---
    public void SpeakSentences(string npcWords)
    {
        if (dialoguePanel == null || dialogueTextLabel == null) return;
        if (isDialogueActive)
        {
            CloseDialogueUI();
            return;
        }
        isDialogueActive = true;
        dialoguePanel.style.display = DisplayStyle.Flex;
        dialogueTextLabel.text = npcWords;
        if (playerMovement != null) playerMovement.use2DMode = false;
    }
    public void CloseDialogueUI()
    {
        if (dialoguePanel == null) return;
        dialoguePanel.style.display = DisplayStyle.None;
        isDialogueActive = false;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        if (playerMovement != null) playerMovement.use2DMode = true;
    }
}