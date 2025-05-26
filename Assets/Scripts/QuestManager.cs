using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
public enum QuestType
{
    KillEnemies,
    CollectItems,
    TalkToNPC,
    KillBoss,
}

[System.Serializable]
public class Quest
{
    public string questName;
    public string questDescription;
    public bool isCompleted;
     public QuestType questType;
    public int targetAmount; 
    public int currentAmount;
    public int goldReward;
    public GameObject npc; 

    public Quest(string name, string description)
    {
        questName = name;
        questDescription = description;
        isCompleted = false;
    }

    public void CompleteQuest()
    {
        isCompleted = true;
        GameManager.Instance.coinsCollected += goldReward;
        if(npc != null)
        {
            
        npc.GetComponent<DialogueTrigger>().ChangeCanvas();
        }
    }
}
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; } 
    public List<Quest> quests;
    public int currentQuestIndex = 0; 
    public Quest currentQuest;
    public Quest givenQuest;
    [SerializeField]
    private ProceduralWeaponPlacement enemyGenerator;

    public TextMeshProUGUI currentQuestDescription;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }
    public void AcceptQuest()
    {
        if (givenQuest != null)
        {
            StartQuest(givenQuest);
        }
    }
    public void StartQuest(Quest quest)
    {
        currentQuest = quest;
        quests.Add(quest);
        UpdateQuestUI();
        givenQuest = null;
        if(quest.questType == QuestType.KillEnemies)
        {
            enemyGenerator.SpawnObjectsNumber(quest.targetAmount);
        }
        else if (quest.questType == QuestType.CollectItems)
        {
            // Logic to spawn items for collection
        }
        else if (quest.questType == QuestType.TalkToNPC)
        {
            // Logic to set up NPC interaction
        }
    }
       public Quest GetCurrentQuest()
    {
        return currentQuest;
    }
      public void UpdateQuestUI()
    {
        if (currentQuestDescription != null)
        {
            if (currentQuest != null)
            {
                if(currentQuest.isCompleted)
                {
                    currentQuestDescription.text = "Quest completed!";
                }
                else
                {
                    currentQuestDescription.text = currentQuest.questDescription;
                }
            }
            
        }
    }
    public void CheckQuestProgress(Quest quest)
    {
        if (quest == null || quest.isCompleted)
            return;

        switch (quest.questType)
        {
            case QuestType.KillEnemies:
                if (quest.currentAmount >= quest.targetAmount)
                    quest.CompleteQuest();
                break;
            case QuestType.CollectItems:
                if (quest.currentAmount >= quest.targetAmount)
                    quest.CompleteQuest();
                break;
            case QuestType.TalkToNPC:
                if (quest.currentAmount >= 1)
                    quest.CompleteQuest();
                break;
        
        }

        UpdateQuestUI();
    }
}
