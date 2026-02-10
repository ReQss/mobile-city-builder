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
    public void StartIntroductionDialogue()
    {
        if(GameManager.Instance.isFirstPlaythrough)
        {
            if(NewDialogueManager.Instance.introductionDialogue != null){
                NewDialogueManager.Instance.StartDialogue(NewDialogueManager.Instance.introductionDialogue);
            }
        }
    }
    void Start()
    {
        // StartIntroductionDialogue();
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
                }
            }
        }
    }
    public NextQuestError AdvanceToNextQuest()
    {
        if ((currentQuestIndex < allQuests.Count - 1) && CurrentQuest.isCompleted == true)
        {
            if(CurrentQuest.currentDialogueIndex < CurrentQuest.questDialogue.Count)
            {
                NewDialogueManager.Instance.StartDialogue(CurrentQuest.questDialogue[CurrentQuest.currentDialogueIndex]);
                CurrentQuest.currentDialogueIndex +=1; 
            }
            currentQuestIndex++;
            if(CurrentQuest.currentDialogueIndex < CurrentQuest.questDialogue.Count)
            {
                NewDialogueManager.Instance.StartDialogue(CurrentQuest.questDialogue[CurrentQuest.currentDialogueIndex]);
                CurrentQuest.currentDialogueIndex +=1;
            }
            return NextQuestError.Completed;
        }
        else if((currentQuestIndex < allQuests.Count - 1) && CurrentQuest.isCompleted == false)
        {
             return NextQuestError.Incompleted;
        }
        else
        {
            Debug.Log("All quests completed!");
            return NextQuestError.AllCompleted;
        }
    }
    
}
