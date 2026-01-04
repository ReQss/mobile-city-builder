using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public GameObject questAcceptPanel;
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
    public GameObject UIDialoguePanel;
    private bool shouldLoadSceneAfterDialogue = false;
    private string sceneToLoadAfterDialogue = null;
    public bool isCurrentDialogueQuest = true;
    private bool isDialogueActive = false;
    
    void Start()
    {
        sentences = new Queue<string>();
        animator.gameObject.SetActive(true);
    }

    public void StartDialogue(Dialogue dialogue, string sceneName, bool isSpecialAction, bool isQuest)
    {
        if (sceneName != null && isSpecialAction)
        {
            this.sceneName = sceneName;
            specialAction = isSpecialAction;
        }
        isDialogueActive = true;
        isCurrentDialogueQuest = isQuest;
        Time.timeScale = 0;
        GameManager.Instance.isPlayerInteracting = true;
        DisableUIElements();
        
        if (UIDialoguePanel != null)
            UIDialoguePanel.SetActive(true);
        animator.gameObject.SetActive(true);
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

        if (UIDialoguePanel != null)
            UIDialoguePanel.SetActive(true);
        Time.timeScale = 0;

        GameManager.Instance.isPlayerInteracting = true;
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
    public void StartDialogueCityChangeScene(Dialogue dialogue, string sceneName, bool isSpecialAction)
    {
        if (UIDialoguePanel != null)
            UIDialoguePanel.SetActive(true);
        GameManager.Instance.isPlayerInteracting = true;
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
        if (sceneName != null)
        {
            shouldLoadSceneAfterDialogue = true;
            sceneToLoadAfterDialogue = sceneName;
        }
    }
    public void StartChoice()
    {
        specialAction = false;
        GameManager.Instance.isPlayerInteracting = true;
        DisableUIElements();
        choiceAnimator.SetBool("IsOpen", true);
    }
    public void DisplayNextSentence()
    {
        if(isDialogueActive == false)return;
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
        if(isDialogueActive == false)return;
        
        animator.SetBool("IsOpen", false);
        GameManager.Instance.isPlayerInteracting = false;
        if (UIDialoguePanel != null)
            UIDialoguePanel.SetActive(false);

        if (shouldLoadSceneAfterDialogue && !string.IsNullOrEmpty(sceneToLoadAfterDialogue))
        {
            shouldLoadSceneAfterDialogue = false;
            SceneManager.LoadScene(sceneToLoadAfterDialogue);
        }
        if (isCurrentDialogueQuest == true)
        {
            if (questAcceptPanel != null)
            {
                questAcceptPanel.SetActive(true);
            }
        }
        isCurrentDialogueQuest = true;
        isDialogueActive = false;
        
    }
   
   
    public void EndChoice()
    {
        choiceAnimator.SetBool("IsOpen", false);
        EnableUIElements();
        GameManager.Instance.isPlayerInteracting = false;
    }
    public void DisableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }
        Time.timeScale = 0;
    }
    public void EnableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            gameObject.SetActive(true);
        }
        Time.timeScale = 1;
    }
}
