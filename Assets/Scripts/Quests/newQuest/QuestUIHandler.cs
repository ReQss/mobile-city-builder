using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> questUIElements;
    public GameObject currentQuestUIElement;
    public TextMeshProUGUI currentQuestUITitle;
    public TextMeshProUGUI currentQuestUIDescription;
    public Image currentQuestUIReward;
    public GameObject completeQuestButton;
    void Start()
    {
        NewQuestManager.Instance.CheckQuestRequirements();
        FillInQuestUIElements(NewQuestManager.Instance.allQuests);
        FillInCurrentQuestUI(NewQuestManager.Instance.CurrentQuest);
        ChangeCompleteQuestButtonState();
    }
    public void ChangeCompleteQuestButtonState()
    {
        completeQuestButton.SetActive(NewQuestManager.Instance.CurrentQuest.isCompleted);
    }
    public void SetQuestButtonsFalse()
    {
        completeQuestButton.SetActive(false);
    }
    public void FirstQuestDialogue()
    {
        if(!NewQuestManager.Instance.isFirstQuestReceived) {
            NewQuestManager.Instance.isFirstQuestReceived = true;
            QuestData CurrentQuest = NewQuestManager.Instance.CurrentQuest;
            NewDialogueManager.Instance.StartDialogue(CurrentQuest.questDialogue[CurrentQuest.currentDialogueIndex]);
            CurrentQuest.currentDialogueIndex +=1;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void CompleteQuest()
    {
        NextQuestError questValid = NewQuestManager.Instance.AdvanceToNextQuest();
        if(questValid == NextQuestError.Completed ){
            QuestData CurrentQuest = NewQuestManager.Instance.CurrentQuest;
            FillInQuestUIElements(NewQuestManager.Instance.allQuests);
            FillInCurrentQuestUI(CurrentQuest);
            NewQuestManager.Instance.CheckQuestRequirements();
            ChangeCompleteQuestButtonState();
            // start quest
            // NewDialogueManager.Instance.StartDialogue(CurrentQuest.questDialogue[CurrentQuest.currentDialogueIndex]);
            // CurrentQuest.currentDialogueIndex +=1;
        }
        else if(questValid == NextQuestError.AllCompleted)
        {
            FillInQuestUIElements(null);
            FillInCurrentQuestUI(null);
            SetQuestButtonsFalse();
        }
        // receive rewards

    }
    public void FillInQuestUIElements(List<QuestData> quests)
    {
        if(quests == null)
        {
            for (int i = 0; i < questUIElements.Count; i++)
            {
                questUIElements[i].SetActive(false);
            }
            return ;
        }
        for (int i = 0; i < questUIElements.Count; i++)
        {
            if (i  + NewQuestManager.Instance.currentQuestIndex < quests.Count)
            {
                // Assuming each quest UI element has a QuestUI component
                questUIElements[i].SetActive(true);
                questUIElements[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quests[i + NewQuestManager.Instance.currentQuestIndex].questName;
                     questUIElements[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
                if(quests[i] == NewQuestManager.Instance.CurrentQuest)
                {
                    // questUIElements[i].transform.GetChild(0).GetComponent<
                }
            }
            else
            {
                questUIElements[i].SetActive(false);
            }
        }
    }
    public void FillInCurrentQuestUI(QuestData quest)
    {
        if (quest == null)
        {
            currentQuestUITitle.text = "All quests completed";
            currentQuestUIDescription.text = "";
            return ;
        }
        // Assuming there's a specific UI element for the current quest
        QuestData currentQuest =  NewQuestManager.Instance.CurrentQuest;
        currentQuestUITitle.text = currentQuest.questName;
        currentQuestUIDescription.text = currentQuest.questDescription;

        
    }
}
