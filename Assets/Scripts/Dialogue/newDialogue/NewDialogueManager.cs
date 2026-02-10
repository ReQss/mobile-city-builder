using System.Collections; // ✅ REQUIRED
using System.Collections.Generic;
using UnityEngine;

public class NewDialogueManager : MonoBehaviour
{
    public static NewDialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;
    
    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private System.Action onDialogueEnd; 
    public DialogueData introductionDialogue; 
    public DialogueManager oldDialogueManagerInstance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (dialogueUI == null)
        {
            dialogueUI = FindObjectOfType<DialogueUI>();
            if (dialogueUI == null)
            {
                Debug.LogError("DialogueUI nie został znaleziony! Dodaj DialogueUI do sceny.");
            }
        }
    }
   
    
    
    public void DisplayNextSentence()
    {
        if (isDialogueActive)
        {
            if (dialogueUI != null && dialogueUI.IsTyping())
                {
                    dialogueUI.CompleteTyping();
                }
                else
                {
                    NextLine();
                }
        }
    }
    public Animator animator;
    public void SetDialogueUI()
    {
        // Time.timeScale = 0;
        // GameManager.Instance.isPlayerInteracting = true;
        // DisableUIElements();

        if (oldDialogueManagerInstance.UIDialoguePanel != null)
            oldDialogueManagerInstance.UIDialoguePanel.SetActive(true);
        animator.gameObject.SetActive(true);
        animator.SetBool("IsOpen", true);
    }
    public void StartDialogue(DialogueData dialogue, System.Action onEnd = null)
    {
        if (dialogue == null || dialogue.dialogueLines == null || dialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("Próba rozpoczęcia pustego dialogu!");
            return;
        }
        if(PlayerMovement.playerMovementInstance != null)
        {
            PlayerMovement.playerMovementInstance.animator.SetBool("Running",false);
        }
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        onDialogueEnd = onEnd;
        // SetDialogueUI();
        // Zatrzymaj ruch gracza podczas dialogu
        if (PlayerMovement.playerMovementInstance != null)
        {
            PlayerMovement.playerMovementInstance.isMovementLocked = false;
        }

        // Pokaż UI dialogu
        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogueUI();
            DisplayCurrentLine();
        }
        else
        {
            Debug.LogError("DialogueUI nie jest przypisany!");
        }
    }

    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || dialogueUI == null) return;

        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = currentDialogue.dialogueLines[currentLineIndex];
        
        // Użyj obrazu z linii dialogowej lub domyślnego
        Sprite portraitToShow = currentLine.characterPortrait != null 
            ? currentLine.characterPortrait 
            : currentDialogue.defaultPersonPortrait;

        // Użyj imienia z linii lub domyślnego
        string nameToShow = !string.IsNullOrEmpty(currentLine.characterName) 
            ? currentLine.characterName 
            : currentDialogue.ghostName;

        dialogueUI.DisplayDialogue(currentLine.text, nameToShow, portraitToShow);
    }

    public void NextLine()
    {
        if (!isDialogueActive || currentDialogue == null) return;

        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        currentDialogue = null;
        currentLineIndex = 0;

        // Przywróć ruch gracza
        if (PlayerMovement.playerMovementInstance != null)
        {
            PlayerMovement.playerMovementInstance.enabled = true;
        }

        
        // Ukryj UI dialogu
        if (dialogueUI != null)
        {
            dialogueUI.HideDialogueUI();
        }

        // Wywołaj callback jeśli jest ustawiony
        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
            onDialogueEnd = null;
        }
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
   
}
