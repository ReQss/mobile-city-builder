using System.Collections.Generic;
using UnityEngine;

public class Quest_CollectItemTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    QuestManager questManager;
    public Quest currentQuest;
    public float distanceLeft = 0f;
    public bool destroyObject = false;
    int dialogueIndex = 0;
    public List<Dialogue> dialogue;
    void Start()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("QuestManager instance not found. Make sure it is initialized in the scene.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        currentQuest = questManager.GetCurrentQuest();
        if (currentQuest != null && currentQuest.questType == QuestType.CollectItems && currentQuest.itemToCollect == this.gameObject)
        {
            distanceLeft = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
            if (distanceLeft < 2.5f)
            {
                if (GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.triggered)
                {
                    Debug.Log("Player is close enough to talk to the NPC.");
                    QuestManager.Instance.currentQuest.currentAmount = currentQuest.targetAmount;
                    QuestManager.Instance.currentQuest.npc = QuestManager.Instance.currentQuest.refToThisNpc;
                    questManager.CheckQuestProgress(currentQuest);
                    // CollectItem();
                    if(destroyObject)
                    Destroy(this.gameObject);
                    if (dialogue.Count > 0)
                    {
                        TriggerDialogue();
                    
                }
                }
               
            }
        }
    }
     public void TriggerDialogue()
    {
         
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue[dialogueIndex],null, false,true);
        dialogueIndex++;
    }
}