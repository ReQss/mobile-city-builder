using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public AudioSource dialogueSound;
    public Animator animator;
    public Animator choiceAnimator;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image image;
    private Queue<string> sentences;
    [SerializeField]
    public GameObject[] uiToDisable;
    public bool specialAction = false;
    public string sceneName = null;
    public bool haveChoice = false;
    public GameObject UIDialoguePanel;    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue, string sceneName, bool isSpecialAction)
    {
        if (sceneName != null && isSpecialAction)
        {
            this.sceneName = sceneName;
            specialAction = isSpecialAction;
        }
        if(UIDialoguePanel != null)
        UIDialoguePanel.SetActive(true);
        GameManager.Instance.isPlayerInteracting = true;
        // Cursor.lockState = CursorLockMode.None;
        DisableUIElements();
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        if (dialogue.npcImage != null)
            image.sprite = dialogue.npcImage;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void StartDialogueCity(Dialogue dialogue, string sceneName, bool isSpecialAction)
    {
       
        if(UIDialoguePanel != null)
        UIDialoguePanel.SetActive(true);
        GameManager.Instance.isPlayerInteracting = true;
        // Cursor.lockState = CursorLockMode.None;
        DisableUIElements();
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name;
        if (dialogue.npcImage != null)
            image.sprite = dialogue.npcImage;
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void StartChoice()
    {
        specialAction = false;
        GameManager.Instance.isPlayerInteracting = true;
        // Cursor.lockState = CursorLockMode.None;
        DisableUIElements();
        choiceAnimator.SetBool("IsOpen", true);
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if(dialogueSound != null)
            dialogueSound.enabled = true;
            dialogueText.text += letter;
            yield return null;
        }
        
            if(dialogueSound != null)
        dialogueSound.enabled = false;
    }
    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        EnableUIElements();
        // Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.isPlayerInteracting = false;
        if(UIDialoguePanel != null)
        UIDialoguePanel.SetActive(false);
      
    }
   
   
    public void EndChoice()
    {
        choiceAnimator.SetBool("IsOpen", false);
        EnableUIElements();
        // Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.isPlayerInteracting = false;
    }
    public void DisableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            if(gameObject != null)
            gameObject.SetActive(false);
        }
    }
    public void EnableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            gameObject.SetActive(true);
        }
    }
}
