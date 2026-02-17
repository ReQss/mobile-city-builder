using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Statistics
{
    [SerializeField]
    private int shortDungeonFinished = 0;
    [SerializeField]
    private int mediumDungeonFinished = 0;
    [SerializeField]
    private int longDungeonFinished = 0;
    [SerializeField]
    private int shortForestFinished = 0;
    [SerializeField]
    private int mediumForestFinished = 0;
    [SerializeField]
    private int longForestFinished = 0;
    [SerializeField]
    private int thugsKilled = 0;
    [SerializeField]
    private int archersKilled = 0;
    [SerializeField]
    private int blackSpidersKilled = 0;
    [SerializeField]
    private int whiteSpidersKilled = 0;
    [SerializeField]
    private int spiderBossKilled = 0;
    public int knightBossKilled = 0;
    public void Add(Statistics other)
    {
        shortDungeonFinished += other.shortDungeonFinished;
        mediumDungeonFinished += other.mediumDungeonFinished;
        longDungeonFinished += other.longDungeonFinished;
        shortForestFinished += other.shortForestFinished;
        mediumForestFinished += other.mediumForestFinished;
        longForestFinished += other.longForestFinished;
        thugsKilled += other.thugsKilled;
        archersKilled += other.archersKilled;
        blackSpidersKilled += other.blackSpidersKilled;
        whiteSpidersKilled += other.whiteSpidersKilled;
        spiderBossKilled += other.spiderBossKilled;
        knightBossKilled += other.knightBossKilled;
    }
      public bool MeetRequirements(Statistics required)
    {
        if (required == null) return true;

        return shortDungeonFinished >= required.shortDungeonFinished
            && mediumDungeonFinished >= required.mediumDungeonFinished
            && longDungeonFinished >= required.longDungeonFinished
            && shortForestFinished >= required.shortForestFinished
            && mediumForestFinished >= required.mediumForestFinished
            && longForestFinished >= required.longForestFinished
            && thugsKilled >= required.thugsKilled
            && archersKilled >= required.archersKilled
            && blackSpidersKilled >= required.blackSpidersKilled
            && whiteSpidersKilled >= required.whiteSpidersKilled
            && spiderBossKilled >= required.spiderBossKilled
            && knightBossKilled >= required.knightBossKilled;
    }
}
public enum NextQuestError
{
    Completed,
    Incompleted,
    AllCompleted
}
public class NewQuestManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public List<QuestData> allQuests;
    public int currentQuestIndex = 0;
    public QuestData CurrentQuest => allQuests[currentQuestIndex];
    public static NewQuestManager Instance { get; private set; }
    public Statistics playerStatistics = new Statistics();
    public bool isFirstQuestReceived = false;
    
    public int currentDialogueIndex = 0;
     void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    
    public void CheckQuestRequirements()
    {
        if (CurrentQuest.statisticsRequired != null)
        {
            // Check if the player statistics meet the quest requirements
            if (playerStatistics != null)
            {
                // Compare player statistics with quest requirements
                if(playerStatistics.MeetRequirements(CurrentQuest.statisticsRequired))
                {
                    CurrentQuest.isCompleted = true;
                    AdvanceToNextQuest();
                }
            }
        }
    }
    
    public void ExecuteQuestActions(List<QuestAction> actions)
    {
        if (actions != null)
        {
            foreach (var action in actions)
            {
                Debug.Log("action made");
                action.Execute();
            }
        }
    }
        public NextQuestError AdvanceToNextQuest()
    {
        currentQuestIndex += 1;
        currentDialogueIndex = 0;

        if (currentQuestIndex >= allQuests.Count)
        {
            Debug.Log("All quests completed!");
            return NextQuestError.AllCompleted;
        }

        QuestData newQuest = CurrentQuest;

        // Wywołanie questActionsStart przy rozpoczęciu nowego questa
        
        ExecuteQuestActions(newQuest.questActionsStart);

        // Jeśli nowy quest ma dialogi, startujemy pierwszy
        if (newQuest.questDialogue != null && newQuest.questDialogue.Count > 0)
        {
            Debug.Log("Starting dialogue for new quest: " + newQuest.questName);
            NewDialogueManager.Instance.StartDialogue(newQuest.questDialogue[currentDialogueIndex]);
            currentDialogueIndex += 1;
        }

        return NextQuestError.Completed;
    }

    
}
