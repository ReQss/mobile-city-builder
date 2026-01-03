using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialoguePanel; // Główny panel dialogu
    [SerializeField] private TextMeshProUGUI dialogueText; // Tekst dialogu
    [SerializeField] private TextMeshProUGUI characterNameText; // Imię postaci
    [SerializeField] private Image characterPortraitImage; // Obraz postaci po lewej stronie
    
    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f; // Szybkość pisania (opcjonalne)
    [SerializeField] private bool useTypewriterEffect = false; // Czy używać efektu pisania

    private bool isTyping = false;
    private string currentDialogueText = "";

    void Start()
    {
        // Ukryj panel dialogu na starcie
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void ShowDialogueUI()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
    }

    public void HideDialogueUI()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        // Zatrzymaj wszystkie efekty
        StopAllCoroutines();
        isTyping = false;
    }

    public void DisplayDialogue(string text, string characterName, Sprite portrait)
    {
        currentDialogueText = text;

        // Ustaw imię postaci
        if (characterNameText != null)
        {
            characterNameText.text = characterName;
        }

        // Ustaw obraz postaci
        if (characterPortraitImage != null)
        {
            if (portrait != null)
            {
                characterPortraitImage.sprite = portrait;
                characterPortraitImage.gameObject.SetActive(true);
            }
            else
            {
                // Jeśli nie ma obrazu, ukryj element (możesz też zostawić domyślny obraz)
                characterPortraitImage.gameObject.SetActive(false);
            }
        }

        // Wyświetl tekst z efektem pisania lub od razu
        if (useTypewriterEffect && dialogueText != null)
        {
            StopAllCoroutines();
            StartCoroutine(TypeText(text));
        }
        else if (dialogueText != null)
        {
            dialogueText.text = text;
        }
    }

    private System.Collections.IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    // Jeśli kliknięto podczas pisania, pokaż cały tekst od razu
    public void CompleteTyping()
    {
        if (isTyping && dialogueText != null)
        {
            StopAllCoroutines();
            dialogueText.text = currentDialogueText;
            isTyping = false;
        }
    }
}

