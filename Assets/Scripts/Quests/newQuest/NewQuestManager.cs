using System.Collections.Generic;
using UnityEngine;
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
     void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        
    }
    public NextQuestError AdvanceToNextQuest()
    {
        if ((currentQuestIndex < allQuests.Count - 1) && CurrentQuest.isCompleted == true)
        {
            currentQuestIndex++;
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
