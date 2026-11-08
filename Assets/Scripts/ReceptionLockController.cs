using UnityEngine;
using UnityEngine.UIElements;

public class ReceptionLockController : MonoBehaviour
{
    private VisualElement lockPanel;
    private TextField codeInput;
    private Button submitButton;
    private PlayerSpriteScript playerMovement;

    // The answer based on your Clues sheet: Rose(04), Henry(07), Clara(10) + Note(3)
    private const string CorrectCode = "4713";

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Fetch UI elements
        lockPanel = root.Q<VisualElement>("ReceptionLockPanel");
        codeInput = root.Q<TextField>("CodeInput");
        submitButton = root.Q<Button>("SubmitCodeButton");

        playerMovement = Object.FindFirstObjectByType<PlayerSpriteScript>();

        if (submitButton != null)
        {
            submitButton.clicked += CheckCombination;
        }
    }

    void OnDisable()
    {
        if (submitButton != null)
        {
            submitButton.clicked -= CheckCombination;
        }
    }

    // Call this method from an interactable object script when pressing 'E' at the reception desk!
    public void OpenReceptionLockUI()
    {
        if (lockPanel == null) return;

        lockPanel.style.display = DisplayStyle.Flex;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        if (codeInput != null)
        {
            codeInput.value = ""; // Clear old inputs
            codeInput.Focus();
        }

        // Put player controller to sleep while entering code
        if (playerMovement != null) playerMovement.use2DMode = false;
    }

    private void CheckCombination()
    {
        if (codeInput == null || lockPanel == null) return;

        if (codeInput.value == CorrectCode)
        {
            Debug.Log("Puzzle Solved! Drawer Unlocked.");

            // --- 🎁 DISPENSE YOUR DESIGN REWARDS ---
            // TODO: Add items directly to your inventory arrays here!
            Debug.Log("Received: Key to Room 2");
            Debug.Log("Received: Key to Room 10");
            Debug.Log("Received: Photograph of Three Guests");
            // ---------------------------------------

            CloseUI();
        }
        else
        {
            Debug.LogWarning("Wrong code! Try analyzing the 13th guest check-ins again.");
            codeInput.value = ""; // Wipe text on failure
            codeInput.Focus();
        }
    }

    public void CloseUI()
    {
        if (lockPanel == null) return;

        lockPanel.style.display = DisplayStyle.None;

        // Wake character movement loop back up
        if (playerMovement != null) playerMovement.use2DMode = true;
    }
}
