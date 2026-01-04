using System.Collections.Generic;
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
    void Start()
    {
        FillInQuestUIElements(NewQuestManager.Instance.allQuests);
        FillInCurrentQuestUI(NewQuestManager.Instance.CurrentQuest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FillInQuestUIElements(List<QuestData> quests)
    {
        for (int i = 0; i < questUIElements.Count; i++)
        {
            if (i < quests.Count)
            {
                // Assuming each quest UI element has a QuestUI component
                questUIElements[i].SetActive(true);
                questUIElements[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quests[i].questName;
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
        // Assuming there's a specific UI element for the current quest
        QuestData currentQuest =  NewQuestManager.Instance.CurrentQuest;
        currentQuestUITitle.text = currentQuest.questName;
        currentQuestUIDescription.text = currentQuest.questDescription;

        
    }
}
