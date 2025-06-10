using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Dialogue> dialogue;
    public string sceneName = null;
    public bool isSpecialAction;
    private Transform player;
    public float detectionRadius = 5f;
    public bool eagleAchievedTrigger = false;
    public bool powerBookTrigger = false;
    [SerializeField]
    public List<Quest> quests;
    public int questIndex = 0;
    public GameObject npcCanvas;
    public GameObject questCanvas;
    public Button specialButton;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        SetNPCQuests();

    }

    public void TriggerDialogue()
    {
        if (quests.Count > 0 && quests.Count > questIndex && (QuestManager.Instance.currentQuest.isCompleted|| questIndex == 0))
        {
            
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue[questIndex], sceneName, isSpecialAction);
            QuestManager.Instance.givenQuest = quests[questIndex];
            questIndex++;
            StartQuestCanvas();
        }
        else
            Debug.Log("Finish your quest first");
    }
    public void TriggerDialogueCity()
    {
        
        FindObjectOfType<DialogueManager>().StartDialogueCity(dialogue[questIndex], sceneName, isSpecialAction);
        questIndex++;
        
    }
    public void TriggerDialogueCityChangeScene(int currentIndex)
    {

        if (currentIndex != TutorialScript.Instance.currentObjectiveIndex)
        {
            return;
        }
        else
        {
            FindObjectOfType<DialogueManager>().StartDialogueCityChangeScene(dialogue[questIndex], "Menu", isSpecialAction);
            questIndex++;
        }
        
    }
    public void TriggerDialogueCityTutorial(int currentIndex)
    {
        if (currentIndex != TutorialScript.Instance.currentObjectiveIndex)
        {
            return;
        }
        else
        {
            FindObjectOfType<DialogueManager>().StartDialogueCity(dialogue[questIndex], sceneName, isSpecialAction);
            questIndex++;
            TutorialScript.Instance.currentObjectiveIndex++;
        }
    }
    public void TriggerDialogueCityTutorialAndEnableButton(int currentIndex)
    {
        if (currentIndex != TutorialScript.Instance.currentObjectiveIndex)
        {
            return;
        }
        else
        {
            FindObjectOfType<DialogueManager>().StartDialogueCity(dialogue[questIndex], sceneName, isSpecialAction);
            questIndex++;
            TutorialScript.Instance.currentObjectiveIndex++;
            TutorialScript.Instance.buttonToUnlock.interactable = true;
        }
    }
    public void SetNPCQuests()
    {
        foreach (var quest in quests)
        {
            if (quest != null)
                quest.npc = this.gameObject;
        }
    }
    public void ChangeCanvas()
    {
        if (npcCanvas != null && questCanvas != null)
        {
            npcCanvas.SetActive(!npcCanvas.activeSelf);
            questCanvas.SetActive(!questCanvas.activeSelf);
        }
    }
    public void StartQuestCanvas()
    {
         if (npcCanvas != null && questCanvas != null)
        {
            npcCanvas.SetActive(false);
            questCanvas.SetActive(true);
        }
    }
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRadius && GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered)
        {
            TriggerDialogue();
        }
        if (GameUIHandler.Instance.cityInteractionAction != null && GameUIHandler.Instance.cityInteractionAction.action.triggered)
        {
            TriggerDialogueCity();
        }
     
    }
}
